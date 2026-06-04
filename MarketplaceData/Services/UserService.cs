using Domain;
using MarketplaceData.Model.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VetClassLibrary.Interfaces;
using VetClassLibrary.Model.User;
using VetClassLibrary.Services;

namespace VetClassLibrary.Services
{
    public class UserService : Repository<UserBase>, IUserService
    {
        public UserService(AppDbContext context) : base(context)
        {
        }
    }
}
