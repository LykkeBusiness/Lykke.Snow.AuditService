// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Lykke.Logs.Serilog;
using Lykke.Snow.Common.Startup;
using Serilog;

namespace Lykke.Snow.AuditService.Startup
{
    internal static class StartupWrapper
    {
        public static Task StartAsync(Func<Task> startAction)
        {
            return StartupLoggingWrapper.HandleStartupException(async () =>
            {
                FailureWrapper.InitializeForHostRestart();

                await FailureWrapper.RetryAsync(startAction, LogStartupException);

            }, serviceShortName: "audit");
        }
        private static void LogStartupException(Exception e, uint attemptLeft)
        {
            Log.Fatal(e, "Host restart initiated. Attempts left: {attemptsLeft}", attemptLeft);
        }
    }
}