namespace VaultHistory.User.Application.Options
{
    public class HashingOptions
    {
        public const string SectionName = "Hashing";
        public int MemorySize { get; set; } = 65536; // 64 MB
        public int DegreeOfParallelism { get; set; } = 8; // Number of threads to use
        public int Iterations { get; set; } = 4; // Number of iterations
        public int SaltSize { get; set; } = 16; // Size of the salt in bytes
        public int HashSize { get; set; } = 32; // Size of the hash in bytes
    }
}