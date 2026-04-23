using Newtonsoft.Json;
namespace VaultHistory.User.Domain.Abstractions
{
    public record Error
    {
        private Error(int code, string type, List<string> details)
        {
            Code = code;
            Type = $"Error.{type}";
            Details = details;
        }
        public int Code { get; }
        public string Type { get; }
        public List<string> Details { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public override string ToString() => $"{Type} - ({Code}): {string.Join(", ", Details)}";
        public string ToJsonString() => JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
        public static Error ReferenceNull => BuildInternalError("ReferenceNull", "The specified reference cannot be null.");
        public static Error None => Build(200, "None", "No error.");
        public static Error BuildInternalError(string type, string detail) => Build(500, $"InternalError.{type}", detail);
        public static Error Build(int code, string type, string detail) => new(code, type, [detail]);
        public static Error Build(int code, string type, List<string> details) => new(code, type, details);
    }
}