using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VaultHistory.User.Infrastructure.Database.ModelBuilders
{
    internal sealed class UserModelBuilder : IEntityTypeConfiguration<Domain.Users.User>
    {
        public void Configure(EntityTypeBuilder<Domain.Users.User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(x => x.Id)
                .IsRequired()
                .HasConversion(
                    id => id.Value,
                    value => new Domain.Users.UserId(value))
                .ValueGeneratedNever();
            
            builder.Property(u => u.IsActive)
                .IsRequired();
            
            builder.Property(u => u.CreatedAt)
                .IsRequired();
            
            builder.Property(u => u.UpdatedAt)
                .IsRequired(false);

            builder.Property(u => u.BirthDate)
                .IsRequired(false);

            builder.OwnsOne(u => u.Email, e =>
            {
                e.Property(p => p.Value)
                    .HasColumnName("Email")
                    .IsRequired();

                e.HasIndex(p => p.Value)
                    .IsUnique();
            });
            
            builder.OwnsOne(u => u.FullName, fn =>
            {
                fn.Property(f => f.FirstName)
                    .HasColumnName("FirstName")
                    .IsRequired();
                
                fn.Property(f => f.LastName)
                    .HasColumnName("LastName")
                    .IsRequired();
            });

            builder.OwnsOne(u => u.Password, p =>
            {
                p.Property(p => p.Hash)
                    .HasColumnName("PasswordHash")
                    .IsRequired();
                
                p.Property(p => p.Salt)
                    .HasColumnName("PasswordSalt")
                    .IsRequired();
            });
        }
    }
}