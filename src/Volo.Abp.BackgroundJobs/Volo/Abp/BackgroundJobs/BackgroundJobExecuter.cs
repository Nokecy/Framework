﻿using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

namespace Volo.Abp.BackgroundJobs
{
    public class BackgroundJobExecuter : IBackgroundJobExecuter, ITransientDependency
    {
        public ILogger<BackgroundJobExecuter> Logger { protected get; set; }

        protected IServiceProvider ServiceProvider { get; }
        protected IClock Clock { get; }
        protected IBackgroundJobSerializer Serializer { get; }
        protected IBackgroundJobStore Store { get; }
        protected BackgroundJobOptions Options { get; }

        public BackgroundJobExecuter(
            IServiceProvider serviceProvider,
            IClock clock,
            IBackgroundJobSerializer serializer,
            IBackgroundJobStore store,
            IOptions<BackgroundJobOptions> options)
        {
            ServiceProvider = serviceProvider;
            Clock = clock;
            Serializer = serializer;
            Options = options.Value;
            Store = store;

            Logger = NullLogger<BackgroundJobExecuter>.Instance;
        }

        public void Execute(BackgroundJobInfo jobInfo)
        {
            try
            {
                jobInfo.TryCount++;
                jobInfo.LastTryTime = Clock.Now;

                var jobType = Options.GetJobType(jobInfo.JobName);

                using (var scope = ServiceProvider.CreateScope())
                {
                    var job = scope.ServiceProvider.GetService(jobType);
                    if (job == null)
                    {
                        throw new AbpException("The job type is not registered to DI: " + jobType);
                    }

                    //TODO: Type check for the job object

                    var jobExecuteMethod = job.GetType().GetMethod("Execute");
                    Debug.Assert(jobExecuteMethod != null, nameof(jobExecuteMethod) + " != null");
                    var argsType = jobExecuteMethod.GetParameters()[0].ParameterType;
                    var argsObj = Serializer.Deserialize(jobInfo.JobArgs, argsType);

                    try
                    {
                        jobExecuteMethod.Invoke(job, new[] { argsObj });
                        AsyncHelper.RunSync(() => Store.DeleteAsync(jobInfo.Id));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);

                        var nextTryTime = jobInfo.CalculateNextTryTime(Clock);
                        if (nextTryTime.HasValue)
                        {
                            jobInfo.NextTryTime = nextTryTime.Value;
                        }
                        else
                        {
                            jobInfo.IsAbandoned = true;
                        }

                        TryUpdate(jobInfo);

                        var backgroundJobException = new BackgroundJobException(
                            "A background job execution is failed. See inner exception for details. See BackgroundJob property to get information on the background job.",
                            ex
                        )
                        {
                            BackgroundJob = jobInfo,
                            JobObject = job
                        };

                        //TODO: Somehow trigger an event for the exception (may create an Volo.Abp.ExceptionHandling package)!
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);

                jobInfo.IsAbandoned = true;

                TryUpdate(jobInfo);
            }
        }

        private void TryUpdate(BackgroundJobInfo jobInfo)
        {
            try
            {
                Store.UpdateAsync(jobInfo);
            }
            catch (Exception updateEx)
            {
                Logger.LogException(updateEx);
            }
        }
    }
}