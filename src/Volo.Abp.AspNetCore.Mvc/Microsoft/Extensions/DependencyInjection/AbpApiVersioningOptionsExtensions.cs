﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Volo.Abp.ApiVersioning;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.AspNetCore.Mvc.Versioning;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AbpApiVersioningOptionsExtensions
    {
        public static void ConfigureAbp(this ApiVersioningOptions options, IServiceCollection services)
        {
            //TODO: Use new builder will be released with Api Versioning 2.1 instead of reflection!

            services.AddTransient<IRequestedApiVersion, HttpContextRequestedApiVersion>();

            services.Configure<AbpAspNetCoreMvcOptions>(op =>
            {
                //TODO: Configuring api version should be done directly inside ConfigureAbp,
                //TODO: not in a callback that will be called by MVC later! For that, we immediately need to controllerAssemblySettings

                foreach (var setting in op.ConventionalControllers.ConventionalControllerSettings)
                {
                    if (setting.ApiVersionConfigurer == null)
                    {
                        ConfigureApiVersionsByConvention(options, setting);
                    }
                    else
                    {
                        setting.ApiVersionConfigurer.Invoke(options);
                    }
                }
            });
        }

        private static void ConfigureApiVersionsByConvention(ApiVersioningOptions options, ConventionalControllerSetting setting)
        {
            foreach (var controllerType in setting.ControllerTypes)
            {
                var controllerBuilder = options.Conventions.Controller(controllerType);

                if (setting.ApiVersions.Any())
                {
                    foreach (var apiVersion in setting.ApiVersions)
                    {
                        controllerBuilder.HasApiVersion(apiVersion);
                    }
                }
                else
                {
                    if (!controllerType.IsDefined(typeof(ApiVersionAttribute), true))
                    {
                        controllerBuilder.IsApiVersionNeutral();
                    }
                }
            }
        }
    }
}
