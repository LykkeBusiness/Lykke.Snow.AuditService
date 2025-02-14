// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using Lykke.Logs.Serilog;
using Lykke.SettingsReader;
using Lykke.SettingsReader.ConfigurationProvider;
using Lykke.Snow.AuditService.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Lykke.Snow.AuditService.Startup
{
    public static class ConfigurationBuilder
    {
        public static (IConfigurationRoot, IReloadingManager<AppSettings>) BuildConfiguration(this WebApplicationBuilder builder)
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyFolder == null)
            {
                throw new DirectoryNotFoundException($"Assembly folder for {Assembly.GetExecutingAssembly().GetName().Name} is not found");
            }
            builder.Environment.ContentRootPath = assemblyFolder;

            var configurationBuilder = builder.Configuration
                .AddSerilogJson(builder.Environment)
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            if (Environment.GetEnvironmentVariable("SettingsUrl")?.StartsWith("http") ?? false)
            {
                configurationBuilder.AddHttpSourceConfiguration();
            }

            var configuration = configurationBuilder.Build();

            var settingsManager = configuration.LoadSettings<AppSettings>(_ => { });

            return (configuration, settingsManager);
        }
    }
}