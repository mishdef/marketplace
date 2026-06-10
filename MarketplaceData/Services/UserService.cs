using Domain;
using MarketplaceData.Model.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VetClassLibrary.Interfaces;
using VetClassLibrary.Model.User;
using VetClassLibrary.Services;

namespace VetClassLibrary.Services
{
    public class UserService : Repository<User>, IUserService
    {
        public UserService(AppDbContext context) : base(context)
        {
        }

        private IQueryable<User> GetQueryWithIncludes()
        {
            return _dbSet
                .Include(u => u.ClientInfo)
                .Include(u => u.SellerInfo)
                .Include(u => u.AdminInfo);
        }

        public override IEnumerable<User> GetAll() => GetQueryWithIncludes().ToList();

        public override async Task<IEnumerable<User>> GetAllAsync() => await GetQueryWithIncludes().ToListAsync();

        public override User? GetById(int id) => GetQueryWithIncludes().FirstOrDefault(u => u.Id == id);

        public override async Task<User?> GetByIdAsync(int id) => await GetQueryWithIncludes().FirstOrDefaultAsync(u => u.Id == id);
    }
}
