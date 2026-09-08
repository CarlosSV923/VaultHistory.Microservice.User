# User service documentation

User is the .NET 10 API for accounts, authentication and the transactional outbox. It follows DDD layers: Domain, Application, Infrastructure and API. A successful sign-in persists `UserSignedInEvent` as a `PENDING` outbox message in the same PostgreSQL transaction.

- Interactive diagram: [User architecture](architecture/user-architecture.html)
- Editable diagram source: [user-architecture.json](architecture/user-architecture.json)
- Local commands: `dotnet restore`, `dotnet build`, `dotnet test`
- Central Docker environment: [Vault.History.System](https://github.com/CarlosSV923/Vault.History.System)

Configuration is read from `src/VaultHistory.User.Api/Configurations` and environment variables. Keep connection strings and JWT keys out of versioned files. User owns EF Core migrations for `users` and `outbox_messages`; Jobs only reads the shared schema through Prisma.
