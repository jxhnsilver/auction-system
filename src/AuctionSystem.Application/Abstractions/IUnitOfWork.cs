using System.Data.Common;

namespace AuctionSystem.Application.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
