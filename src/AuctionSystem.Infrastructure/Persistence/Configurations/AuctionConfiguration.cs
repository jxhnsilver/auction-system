using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Lots;
using AuctionSystem.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSystem.Infrastructure.Persistence.Configurations
{
    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasConversion(
                    id => id.Value,
                    value => AuctionId.From(value))
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(a => a.SellerId)
                .HasConversion(
                    id => id.Value,
                    value => UserId.From(value))
                .IsRequired();

            builder.Property(a => a.LotId)
                .HasConversion(
                    id => id.Value,
                    value => LotId.From(value))
                .IsRequired();

            builder.Property(a => a.StartingPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(a => a.CurrentPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(a => a.StartTime)
                .IsRequired();

            builder.Property(a => a.EndTime)
                .IsRequired();

            builder.HasOne<Lot>()
                .WithMany()
                .HasForeignKey(a => a.LotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.SellerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Bids)
                .WithOne()
                .HasForeignKey(b => b.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(a => a.Bids)
                .HasField("_bids");

            builder.Property<byte[]>("RowVersion")
                .IsRowVersion()
                .IsRequired();
        }
    }
}