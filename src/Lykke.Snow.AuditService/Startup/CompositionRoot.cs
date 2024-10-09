// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.SettingsReader;
using Lykke.Snow.AuditService.MappingProfiles;
using Lykke.Snow.AuditService.Settings;
using Lykke.Snow.Common.AssemblyLogging;
using Lykke.Snow.Common.Startup;
using Lykke.Snow.Common.Startup.ApiKey;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Snow.AuditService.Startup
{
    public static class CompositionRoot
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            if(settings.CurrentValue.AuditService == null)
                throw new ArgumentException($"{nameof(AppSettings.AuditService)} settings is not configured!");

            services.AddAssemblyLogger();
            services.AddSingleton(settings.CurrentValue.AuditService);
            services
                .AddApplicationInsightsTelemetry()
                .AddMvcCore()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddApiExplorer();

            services.AddControllers();
            services.AddAuthorization();
            services.AddApiKeyAuth(settings.CurrentValue.AuditService.AuditServiceClient);

            services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo { Version = "v1", Title = $"{Program.ApiName}" });

                    if (!string.IsNullOrWhiteSpace(settings.CurrentValue.AuditService.AuditServiceClient?.ApiKey))
                        options.AddApiKeyAwareness();
                })
                .AddSwaggerGenNewtonsoftSupport();

            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }

    }
}