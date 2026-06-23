using AuctionSystem.Application.Features.Auctions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuctionSystem.Infrastructure.BackgroundServices
{
    public class ExpiredAuctionCloserWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<ExpiredAuctionCloserWorker> logger
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

                    var closer = scope.ServiceProvider.GetRequiredService<IExpiredAuctionCloser>();

                    await closer.CloseExpiredAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Auction Closer Worker is stopping.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while closing expired auctions.");
            }
        }
    }
}
