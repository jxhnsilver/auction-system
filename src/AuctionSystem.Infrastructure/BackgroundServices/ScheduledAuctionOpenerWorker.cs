using AuctionSystem.Application.Features.Auctions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuctionSystem.Infrastructure.BackgroundServices
{
    public class ScheduledAuctionOpenerWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<ScheduledAuctionOpenerWorker> logger
        ) : BackgroundService
    {
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_checkInterval);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    using var scope = scopeFactory.CreateScope();

                    var auctionOpener = scope.ServiceProvider.GetRequiredService<IScheduledAuctionOpener>();

                    await auctionOpener.OpenScheduledAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Auction Opener Worker is stopping.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while opening scheduled auctions.");
            }
        }
    }
}
