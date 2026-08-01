using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pingly_api.SSE;

namespace pingly_api.services
{
    public class SseHeartbeatWorkers : BackgroundService
    {
        private readonly SseConnectionManager _connectionManager;

        public SseHeartbeatWorkers(
            SseConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }


        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            using var timer =
                new PeriodicTimer(
                    TimeSpan.FromSeconds(20));


            while (await timer.WaitForNextTickAsync(
                stoppingToken))
            {
                var topics =
                    _connectionManager.GetActiveTopic();


                foreach (var topic in topics)
                {
                    await _connectionManager
                        .BroadcastHeartbeatAsync(topic);
                }
            }
        }
    }
}