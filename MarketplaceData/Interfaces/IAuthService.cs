using VetClassLibrary.Model.User;

namespace VetClassLibrary.Interfaces
{
    public interface IAuthService
    {
        Client Login(string username, string password);
    }
}