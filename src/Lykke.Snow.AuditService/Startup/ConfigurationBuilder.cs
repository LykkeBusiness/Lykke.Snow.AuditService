// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;
using Lykke.Logs.Serilog;
using Lykke.SettingsReader;
using Lykke.Snow.AuditService.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Lykke.Snow.AuditService.Startup
{
    public static class ConfigurationBuilder
    {
        public static (IConfigurationRoot, IReloadingManager<AppSettings>) BuildConfiguration(this WebApplicationBuilder builder)
        {
            builder.Environment.ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var configuration = builder.Configuration
                .AddSerilogJson(builder.Environment)
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();

            var settingsManager = configuration.LoadSettings<AppSettings>(_ => { });

            return (configuration, settingsManager);
        }
    }
}