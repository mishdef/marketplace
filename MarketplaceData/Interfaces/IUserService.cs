using Domain;
using MarketplaceData.Model.User;
using VetClassLibrary.Model.User;

namespace VetClassLibrary.Interfaces
{
    public interface IUserService : IRepository<User>
    {
    }
}