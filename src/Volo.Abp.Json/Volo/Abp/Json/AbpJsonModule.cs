﻿using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace Volo.Abp.Json
{
    [DependsOn(typeof(AbpTimingModule))]
    public class AbpJsonModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAssemblyOf<AbpJsonModule>();
        }
    }
}
