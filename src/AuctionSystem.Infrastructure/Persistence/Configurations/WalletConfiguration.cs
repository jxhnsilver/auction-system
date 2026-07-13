using AuctionSystem.Domain.Aggregates.Auctions;
using AuctionSystem.Domain.Aggregates.Users;
using AuctionSystem.Domain.Aggregates.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSystem.Infrastructure.Persistence.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .HasConversion(
                    id => id.Value,
                    value => WalletId.From(value))
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(w => w.OwnerId)
                .HasConversion(
                    id => id.Value,
                    value => UserId.From(value))
                .IsRequired();

            builder.Property(w => w.Balance)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(w => w.LockedBalance)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Ignore(w => w.AvailableBalance);

            builder.HasMany(w => w.Holds)
                .WithOne()
                .HasForeignKey(h => h.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(w => w.Holds)
                .HasField("_holds");

            builder.Property<byte[]>("RowVersion")
                .IsRowVersion()
                .IsRequired();
        }
    }
}