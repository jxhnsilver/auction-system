using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSystem.Infrastructure.Persistence.Configurations
{
    public class WalletHoldConfiguration : IEntityTypeConfiguration<WalletHold>
    {
        public void Configure(EntityTypeBuilder<WalletHold> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .HasConversion(
                    id => id.Value,
                    value => WalletHoldId.From(value))
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(h => h.WalletId)
                .HasConversion(
                    id => id.Value,
                    value => WalletId.From(value))
                .IsRequired();

            builder.Property(h => h.AuctionId)
                .HasConversion(
                    id => id.Value,
                    value => AuctionId.From(value))
                .IsRequired();

            builder.Property(h => h.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(h => h.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(h => h.CreatedAt)
                .IsRequired();

            builder.Property(h => h.CompletedAt)
                .IsRequired(false);
        }
    }
}