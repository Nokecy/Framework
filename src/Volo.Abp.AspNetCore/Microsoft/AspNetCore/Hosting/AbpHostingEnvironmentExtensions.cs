﻿using Microsoft.Extensions.Configuration;

namespace Microsoft.AspNetCore.Hosting
{
    public static class AbpHostingEnvironmentExtensions
    {
        public static IConfigurationRoot BuildConfiguration(this IHostingEnvironment env, string fileName = "appsettings")
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(fileName + ".json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{fileName}.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}
