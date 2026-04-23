namespace VaultHistory.User.Application.Options
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        public string PrivateKey { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;
        public string Issuer { get; set; } = string.Empty; // Typically the application or organization issuing the token
        public string Audience { get; set; } = string.Empty; // Typically the application or service the token is intended for
    }
}