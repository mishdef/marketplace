 using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VetClassLibrary.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VetClassLibrary.Services;
using VetClassLibrary.Model.User;
using VetClassLibrary.Interfaces;
using MarketplaceData.Model.User;

namespace VetAPI.Services
{
    public class ApiUserAuthService : IApiUserAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public ApiUserAuthService(AppDbContext db, IConfiguration configuration, IUserService userService)
        {
            _context = db;
            _configuration = configuration;
            _userService = userService;
        }

        public async Task<bool> DleteUserAsync(int userId)
        {
            try
            {
                await _userService.DeleteAsync(userId);
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete user", ex);
            }
        }

        public async Task<User?> EditUserAsync(int userId, UserRegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with id {userId} not found");
                }
                user.FullName = registrationRequestDTO.FullName;
                user.PhoneNumber = registrationRequestDTO.PhoneNumber;
                user.Password = registrationRequestDTO.Password;
                user.Role = registrationRequestDTO.Role;
                
                await _userService.UpdateAsync(user);

                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to edit user", ex);
            }
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userService.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userService.GetAllAsync();
        }

        public async Task<bool> IsUserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName != null && u.UserName.ToLower() == username.ToLower());
        }

        public async Task<UserLoginResponceDTO?> LoginAsync(UserLoginRequestDTO loginRequestDTO)
        {
            try
            {
                var user = (await _userService.GetAllAsync()).FirstOrDefault(u => u.UserName != null && u.UserName.ToLower() == loginRequestDTO.Username.ToLower());

                if (user == null || user.Password != loginRequestDTO.Password)
                {
                    return null;
                }

                var token = GenerateJwtToken(user);

                return new UserLoginResponceDTO()
                {
                    User = user,
                    Token = token
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to login the user", ex);
            }
        }

        public async Task<User?> RegisterAsync(UserRegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                if (await IsUserExistsAsync(registrationRequestDTO.PhoneNumber))
                {
                    throw new InvalidOperationException($"User with phone number {registrationRequestDTO.PhoneNumber} already exists");
                }

                var dto = registrationRequestDTO;

                User user = new User();
                
                if (dto.Role == "Admin") {
                    user.AdminInfo = new AdminInfo();
                } else if (dto.Role == "Client") {
                    user.ClientInfo = new ClientInfo();
                } else if (dto.Role == "Seller") {
                    user.SellerInfo = new SellerInfo();
                } else {
                    throw new InvalidOperationException($"Invalid role {dto.Role}");
                }

                user.FullName = dto.FullName;
                user.PhoneNumber = dto.PhoneNumber;
                user.Password = dto.Password;
                user.Role = dto.Role;
                user.UserName = dto.PhoneNumber;
                user.NormalizedUserName = dto.PhoneNumber.ToUpper();
                user.SecurityStamp = Guid.NewGuid().ToString();

                await _userService.CreateAsync(user);

                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to register user", ex);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings")["Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(90),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
