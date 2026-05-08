namespace VaultHistory.User.Infrastructure.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public bool Processed { get; set; } = false;
        public DateTime? ProcessedOn { get; set; }
        public string? Error { get; set; }

    }
    
}