﻿using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Volo.Abp.UI
{
    [DependsOn(
        typeof(AbpLocalizationModule)
    )]
    public class AbpUiModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Configure<VirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpUiModule>();
            });

            context.Services.Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources.Add<AbpUiResource>("en").AddVirtualJson("/Localization/Resources/AbpUi");
            });
            
            context.Services.AddAssemblyOf<AbpUiModule>();
        }
    }
}
