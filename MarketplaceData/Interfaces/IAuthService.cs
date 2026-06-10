using VetClassLibrary.Model.User;
using MarketplaceData.Model.User;

namespace VetClassLibrary.Interfaces
{
    public interface IAuthService
    {
        User Login(string username, string password);
    }
}