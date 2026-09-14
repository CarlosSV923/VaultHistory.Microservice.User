using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VaultHistory.User.Infrastructure.Database.ModelBuilders
{
    internal sealed class UserModelBuilder : IEntityTypeConfiguration<Domain.Users.User>
    {
        public void Configure(EntityTypeBuilder<Domain.Users.User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(u => u.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .HasConversion(
                    id => id.Value,
                    value => new Domain.Users.UserId(value))
                .ValueGeneratedNever();
            
            builder.Property(u => u.IsActive)
                .HasColumnName("isActive")
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(u => u.CreatedAt)
                .HasColumnName("createdAt")
                .HasPrecision(3)
                .IsRequired();
            
            builder.Property(u => u.UpdatedAt)
                .HasColumnName("updatedAt")
                .HasPrecision(3)
                .IsRequired(false);

            builder.Property(u => u.BirthDate)
                .HasColumnName("birthDate")
                .HasConversion(
                    date => date.HasValue
                        ? DateTime.SpecifyKind(date.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
                        : (DateTime?)null,
                    date => date.HasValue ? DateOnly.FromDateTime(date.Value) : null)
                .HasColumnType("timestamp(3) with time zone")
                .IsRequired(false);

            builder.Property(u => u.Notification)
                .HasColumnName("notification")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.NotificationStatus)
                .HasColumnName("notificationStatus");

            builder.Property(u => u.NotificationDate)
                .HasColumnName("notificationDate")
                .HasPrecision(3);

            builder.Property(u => u.Theme)
                .HasColumnName("theme");

            builder.Property(u => u.Character)
                .HasColumnName("character");

            builder.Property(u => u.FullName)
                .HasColumnName("fullname")
                .HasConversion(
                    fullName => fullName.GetFullName(),
                    value => Domain.Users.FullName.FromPersistedValue(value))
                .IsRequired();

            builder.OwnsOne(u => u.Email, e =>
            {
                e.Property(p => p.Value)
                    .HasColumnName("email")
                    .IsRequired();

                e.HasIndex(p => p.Value)
                    .IsUnique();
            });
            
            builder.OwnsOne(u => u.Password, p =>
            {
                p.Property(p => p.Hash)
                    .HasColumnName("passwordHash")
                    .IsRequired();
                
                p.Property(p => p.Salt)
                    .HasColumnName("passwordSalt")
                    .IsRequired();
            });
        }
    }
}
