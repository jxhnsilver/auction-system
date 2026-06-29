using AuctionSystem.Domain.Auctions;
using AuctionSystem.Domain.Primitives;
using Microsoft.Extensions.Logging;

namespace AuctionSystem.Application.Features.Auctions.Services
{
    public class ScheduledAuctionOpener(
        IAuctionRepository auctionRepository,
        IClock clock,
        ILogger<ScheduledAuctionOpener> logger
        ) : IScheduledAuctionOpener
    {
        private int BatchSize = 25;
        public async Task OpenScheduledAsync(CancellationToken cancellationToken)
        {
            var now = clock.UtcNow;

            var scheduledAuctionIds = await auctionRepository.GetScheduledIdsAsync(now, cancellationToken);

            if (scheduledAuctionIds.Count == 0)
                return;

            int totalOpened = 0;

            for (int i = 0; i < scheduledAuctionIds.Count; i += BatchSize)
            {
                int currentBatchSize = Math.Min(BatchSize, scheduledAuctionIds.Count - i);

                var batch = new AuctionId[currentBatchSize];

                for (int j = 0; j < batch.Length; j++)
                {
                    if (i + j < scheduledAuctionIds.Count)
                    {
                        batch[j] = scheduledAuctionIds[i + j];
                    }
                }
                try
                {
                    totalOpened += await auctionRepository.OpenBatchAsync(batch, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process auction opening batch with {Count} auctions", batch.Length);
                }
            }

            logger.LogInformation("Opend {Count}/{Total} scheduled auctions", totalOpened, scheduledAuctionIds.Count);
        }
    }
}
