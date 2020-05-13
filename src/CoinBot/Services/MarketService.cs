using System;
using System.Threading;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Hosting;

namespace CoinBot.Services
{
    public sealed class MarketService : BackgroundService
    {
        private readonly MarketManager _marketManager;

        public MarketService(MarketManager marketManager)
        {
            this._marketManager = marketManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._marketManager.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(value: 30), cancellationToken: stoppingToken);
            }

            this._marketManager.Stop();
        }
    }
}