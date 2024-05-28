// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Lykke.Snow.AuditService.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Snow.AuditService
{
    internal sealed class Program
    {
        public const string ApiName = "Audit";

        public static async Task Main(string[] args)
        {
            await StartupWrapper.StartAsync(async() => 
            {
                var builder = WebApplication.CreateBuilder(args);
                var (configuration, settingsManager) = builder.BuildConfiguration();

                builder.Services.RegisterInfrastructureServices(settingsManager);

                builder.ConfigureHost(configuration, settingsManager);

                var app = builder.Build();

                await app.Configure().RunAsync();
            });
        }
    }
}
