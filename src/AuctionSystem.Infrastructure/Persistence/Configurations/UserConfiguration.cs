using AuctionSystem.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSystem.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasConversion(
                    id => id.Value,
                    value => UserId.From(value))
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(u => u.Email)
                .HasConversion(
                    email => email.Value,
                    value => Email.Create(value).Value)
                .HasMaxLength(Email.MaxTotalLength)
                .IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();
        }
    }
}
