using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VaultHistory.User.Infrastructure.Outbox;

namespace VaultHistory.User.Infrastructure.Database.ModelBuilders
{
    internal sealed class OutboxMessageBuilder : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(m => m.Type)
                .HasColumnName("type")
                .IsRequired();

            builder.Property(m => m.Payload)
                .HasColumnName("payload")
                .IsRequired();

            builder.Property(m => m.OccurredOn)
                .HasColumnName("occurredOn")
                .HasPrecision(3)
                .IsRequired();

            builder.Property(m => m.Status)
                .HasColumnName("status")
                .HasDefaultValue("PENDING");

            builder.Property(m => m.UpdateAt)
                .HasColumnName("updateAt")
                .HasPrecision(3);

            builder.Property(m => m.Error)
                .HasColumnName("error");
        }
    }
}
