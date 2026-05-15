namespace VaultHistory.User.Application.Providers.UserContext
{
    public interface IUserContextProvider
    {
        string GetUserId();
        string GetUserEmail();
        string GetUserFullName();
    }
}