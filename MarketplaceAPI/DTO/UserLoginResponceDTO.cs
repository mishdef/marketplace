using MarketplaceData.Model.User;
using VetClassLibrary.Model.User;

namespace VetClassLibrary.DTO
{
    public class UserLoginResponceDTO
    {
        public string? Token { get; set; }

        public UserBase? User { get; set; }
    }
}
