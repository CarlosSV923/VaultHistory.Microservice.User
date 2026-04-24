using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VaultHistory.User.Infrastructure.Outbox;

namespace VaultHistory.User.Infrastructure.Database.ModelBuilders
{
    internal sealed class OutboxMessageBuilder : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Type)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(m => m.Payload)
                .IsRequired();

            builder.Property(m => m.OccurredOn)
                .IsRequired();

            builder.Property(m => m.Processed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.ProcessedOn);

            builder.Property(m => m.Error);
        }
    }
}