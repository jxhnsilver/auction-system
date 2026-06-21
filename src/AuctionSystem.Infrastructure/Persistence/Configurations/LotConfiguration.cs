using AuctionSystem.Domain.Lots;
using AuctionSystem.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSystem.Infrastructure.Persistence.Configurations
{
    public class LotConfiguration : IEntityTypeConfiguration<Lot>
    {
        public void Configure(EntityTypeBuilder<Lot> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .HasConversion(
                    id => id.Value,
                    value => LotId.From(value))
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(l => l.OwnerId)
                .HasConversion(
                    id => id.Value,
                    value => UserId.From(value))
                .IsRequired();

            builder.Property(l => l.Title)
                .IsRequired();

            builder.Property(l => l.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
        }
    }
}