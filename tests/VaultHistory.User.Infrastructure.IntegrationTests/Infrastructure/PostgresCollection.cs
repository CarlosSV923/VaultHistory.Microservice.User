namespace VaultHistory.User.Infrastructure.IntegrationTests.Infrastructure;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "Infrastructure.Postgres";
}
