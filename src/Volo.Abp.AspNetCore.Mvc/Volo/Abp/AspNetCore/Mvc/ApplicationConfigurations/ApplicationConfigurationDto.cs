﻿namespace Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations
{
    public class ApplicationConfigurationDto
    {
        public ApplicationLocalizationConfigurationDto Localization { get; set; }

        public ApplicationAuthConfigurationDto Auth { get; set; }
    }
}