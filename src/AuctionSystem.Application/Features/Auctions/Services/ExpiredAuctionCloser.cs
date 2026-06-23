using AuctionSystem.Application.Abstractions;
using AuctionSystem.Domain.Auctions;
using Microsoft.Extensions.Logging;

namespace AuctionSystem.Application.Features.Auctions.Services
{
    public class ExpiredAuctionCloser(
        IAuctionRepository auctionRepository,
        IClock clock,
        ILogger<ExpiredAuctionCloser> logger
        ) : IExpiredAuctionCloser
    {
        private const int BatchSize = 25;
        public async Task CloseExpiredAsync(CancellationToken cancellationToken)
        {
            var now = clock.UtcNow;

            var expiredAuctionIds = await auctionRepository.GetExpiredIdsAsync(now, cancellationToken);

            if (expiredAuctionIds.Count == 0)
                return;

            int totalClosed = 0;

            for (int i = 0; i < expiredAuctionIds.Count; i += BatchSize)
            {
                int currentBatchSize = Math.Min(BatchSize, expiredAuctionIds.Count - i);

                var batch = new AuctionId[currentBatchSize];

                for (int j = 0; j < batch.Length; j++)
                {
                    if (i + j < expiredAuctionIds.Count)
                    {
                        batch[j] = expiredAuctionIds[i + j];
                    }
                }

                try
                {
                    totalClosed += await auctionRepository.CloseBatchAsync(batch, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process auction closing batch with {Count} auctions", batch.Length);
                }
            }

            logger.LogInformation("Closed {Count}/{Total} expired auctions.", totalClosed, expiredAuctionIds.Count);
        }
    }
}
