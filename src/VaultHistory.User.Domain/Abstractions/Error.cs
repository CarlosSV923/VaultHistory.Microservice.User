namespace VaultHistory.User.Domain.Abstractions
{
    public record Error(string Code, string Message)
    {
        public static Error None => new("None", "No error");
        public static Error NullValue => new("NullValue", "A required value was null");
    }
}