// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.Snow.Common.Startup.ApiKey;

namespace Lykke.Snow.AuditService.Settings
{
    public class AuditServiceSettings
    {
        public DbSettings Db { get; set; } = new DbSettings();
        public ClientSettings? AuditServiceClient { get; set; }
    }
}