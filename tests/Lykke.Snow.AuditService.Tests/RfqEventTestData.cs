// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

using MarginTrading.Backend.Contracts.Events;

namespace Lykke.Snow.AuditService.Tests
{
    internal class RfqEventTestData : IEnumerable<object[]>
    {
        public const string BrokerId = "Consors";
        
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] 
            {
                new RfqEvent
                {
                    EventType = RfqEventTypeContract.New,
                    BrokerId = BrokerId,
                    RfqSnapshot = new MarginTrading.Backend.Contracts.Rfq.RfqContract()
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}