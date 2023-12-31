// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.RabbitMqBroker;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Snow.AuditService.Settings
{
    public class SubscriptionSettings
    {
        [Optional]
        public string? RoutingKey { get; set; }

        [Optional]
        public bool IsDurable { get; set; }

        public string? ExchangeName { get; set; }

        public string? QueueName { get; set; }

        [AmqpCheck]
        public string? ConnectionString { get; set; }

        public static implicit operator RabbitMqSubscriptionSettings(SubscriptionSettings boSettings)
        {
            return new RabbitMqSubscriptionSettings
            {
                ConnectionString = boSettings.ConnectionString,
                ExchangeName = boSettings.ExchangeName,
                IsDurable = boSettings.IsDurable,
                QueueName = boSettings.QueueName,
                RoutingKey = boSettings.RoutingKey
            };
        }
    }
}
