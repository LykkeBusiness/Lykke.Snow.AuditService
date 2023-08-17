// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Lykke.RabbitMqBroker;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.AuditService.Startup
{
    public class StartupManager
    {
        private readonly IEnumerable<IStartStop> _startables;
        private readonly ILogger<StartupManager> _logger;

        public StartupManager(IEnumerable<IStartStop> startables, 
            ILogger<StartupManager> logger)
        {
            _startables = startables;
            _logger = logger;
        }

        internal void Start()
        {
            foreach(var component in _startables)
            {
                StartComponent(component);
            }
        }
        
        private void StartComponent(IStartStop component)
        {
            try 
            {
                component.Start();
                
                _logger.LogInformation("Started {Component} successfully.", component.GetType().Name);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Couldn't start the component {Component}", component.GetType().Name);

                throw;
            }
        }
    }
}