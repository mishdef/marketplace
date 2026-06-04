using MarketplaceData.Model.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VetClassLibrary.DTO;
using VetClassLibrary.Model.User;

namespace VetAPI.Services
{
    public interface IApiUserAuthService
    {
        public Task<bool> DleteUserAsync(int userId);

        public Task<UserBase?> EditUserAsync(int userId, UserRegistrationRequestDTO registrationRequestDTO);

        public Task<IEnumerable<UserBase>> GetUsers();

        public Task<IEnumerable<UserBase>> GetUsersAsync();

        public Task<bool> IsUserExistsAsync(string username);

        public Task<UserLoginResponceDTO?> LoginAsync(UserLoginRequestDTO loginRequestDTO);

        public Task<UserBase?> RegisterAsync(UserRegistrationRequestDTO registrationRequestDTO);
    }
}
