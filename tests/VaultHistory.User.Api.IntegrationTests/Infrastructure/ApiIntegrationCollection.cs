namespace VaultHistory.User.Api.IntegrationTests.Infrastructure;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ApiIntegrationCollection : ICollectionFixture<ApiIntegrationFixture>
{
    public const string Name = "Api.Integration";
}
