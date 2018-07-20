﻿using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Localization.Resources.AbpValidation;
using Volo.Abp.Modularity;
using Volo.Abp.Settings;
using Volo.Abp.VirtualFileSystem;

namespace Volo.Abp.Localization
{
    [DependsOn(
        typeof(AbpVirtualFileSystemModule),
        typeof(AbpSettingsModule)
        )]
    public class AbpLocalizationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AbpStringLocalizerFactory.Replace(context.Services);

            context.Services.Configure<VirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AbpLocalizationModule>("Volo.Abp", "Volo/Abp");
            });

            context.Services.Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                .Add<AbpValidationResource>("en")
                .AddVirtualJson("/Localization/Resources/AbpValidation");
            });

            context.Services.Configure<SettingOptions>(options =>
            {
                options.DefinitionProviders.Add<LocalizationSettingProvider>();
            });

            context.Services.AddAssemblyOf<AbpLocalizationModule>();
        }
    }
}
