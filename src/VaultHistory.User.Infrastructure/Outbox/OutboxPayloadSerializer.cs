using Newtonsoft.Json;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users.Events;

namespace VaultHistory.User.Infrastructure.Outbox;

public static class OutboxPayloadSerializer
{
    public static string Serialize(IDomainEvent domainEvent) => domainEvent switch
    {
        CreateUserEvent created => JsonConvert.SerializeObject(new { userId = created.UserId.Value }),
        UserSignedInEvent signedIn => JsonConvert.SerializeObject(new { userId = signedIn.UserId.Value }),
        _ => JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        })
    };
}
