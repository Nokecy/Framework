﻿using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Serialization;
using Volo.Abp.Threading;

namespace Volo.Abp.Caching
{
    [DependsOn(typeof(AbpThreadingModule))]
    [DependsOn(typeof(AbpSerializationModule))]
    [DependsOn(typeof(AbpMultiTenancyAbstractionsModule))]
    public class AbpCachingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMemoryCache();
            context.Services.AddDistributedMemoryCache();

            context.Services.AddAssemblyOf<AbpCachingModule>();

            context.Services.AddSingleton(typeof(IDistributedCache<>), typeof(DistributedCache<>));
        }
    }
}
