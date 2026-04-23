namespace VaultHistory.User.Application.Options
{
    public class PasswordOptions
    {
        public const string SectionName = "Password";
        public int MinLength { get; set; } = 8;
        public string Regex { get; set; }  = string.Empty;
    }
}