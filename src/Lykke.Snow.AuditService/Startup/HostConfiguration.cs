// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.SettingsReader;
using Lykke.Snow.AuditService.Extensions;
using Lykke.Snow.AuditService.Modules;
using Lykke.Snow.AuditService.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Lykke.Snow.AuditService.Startup
{
    public static class HostConfiguration
    {
        public static IHostBuilder ConfigureHost(this WebApplicationBuilder builder, IConfiguration configuration, IReloadingManager<AppSettings> settings)
        {
            if(settings.CurrentValue.AuditService == null)
                throw new ArgumentException($"{nameof(AppSettings.AuditService)} settings is not configured!");

            if(settings.CurrentValue.AuditService.Db == null)
                throw new ArgumentNullException(nameof(settings.CurrentValue.AuditService.Db));

            if(settings.CurrentValue.AuditService.Db.ConnectionString == null)
                throw new ArgumentNullException(nameof(settings.CurrentValue.AuditService.Db.ConnectionString));

            var hostBuilder = builder.Host
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((ctx, cBuilder) =>
                {
                    // register Autofac modules here
                    cBuilder.RegisterModule(new ServiceModule());
                })
                .UseSerilog((_, cfg) =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var assembly = typeof(Program).Assembly;
                    var title = assembly.Attribute<AssemblyTitleAttribute>(attribute => attribute.Title);
                    var version = assembly.Attribute<AssemblyInformationalVersionAttribute>(attribute => attribute.InformationalVersion);
                    var copyright = assembly.Attribute<AssemblyCopyrightAttribute>(attribute => attribute.Copyright);

                    cfg.ReadFrom.Configuration(configuration)
                        .Enrich.WithProperty("Application", title)
                        .Enrich.WithProperty("Version", version)
                        .Enrich.WithProperty("Environment", environmentName ?? "Development");
                    
                     Log.Information($"{title} [{version}] {copyright}");
                     Log.Information($"Running on: {RuntimeInformation.OSDescription}");
                });

            
            return hostBuilder;
        }
    }
}