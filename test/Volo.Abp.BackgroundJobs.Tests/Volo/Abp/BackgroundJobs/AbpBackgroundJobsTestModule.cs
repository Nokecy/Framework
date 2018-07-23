﻿using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Volo.Abp.BackgroundJobs
{
    [DependsOn(
        typeof(AbpBackgroundJobsModule),
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule)
    )]
    public class AbpBackgroundJobsTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAssemblyOf<AbpBackgroundJobsTestModule>();
        }
    }
}