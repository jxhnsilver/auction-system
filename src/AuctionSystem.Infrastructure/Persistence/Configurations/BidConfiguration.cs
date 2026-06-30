using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSystem.Infrastructure.Persistence.Configurations
{
    public class BidConfiguration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .HasConversion(
                    id => id.Value,
                    value => BidId.From(value))
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(b => b.AuctionId)
                .HasConversion(
                    id => id.Value,
                    value => AuctionId.From(value))
                .IsRequired();

            builder.Property(b => b.BidderId)
                .HasConversion(
                    id => id.Value,
                    value => UserId.From(value))
                .IsRequired();

            builder.Property(b => b.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(b => b.PlacedAt)
                .IsRequired();
        }
    }
}