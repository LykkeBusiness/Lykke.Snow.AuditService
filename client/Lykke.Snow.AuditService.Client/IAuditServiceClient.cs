// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using JetBrains.Annotations;

namespace Lykke.Snow.AuditService.Client
{
    /// <summary>
    /// AuditService client interface.
    /// </summary>
    [PublicAPI]
    public interface IAuditServiceClient
    {
        // Make your app's controller interfaces visible by adding corresponding properties here.
        // NO actual methods should be placed here (these go to controller interfaces, for example - IAuditServiceApi).
        // ONLY properties for accessing controller interfaces are allowed.

        /// <summary>Application Api interface</summary>
        IAuditServiceApi Api { get; }
    }
}
