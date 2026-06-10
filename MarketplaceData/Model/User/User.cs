using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using VetClassLibrary.Model.User;

namespace MarketplaceData.Model.User
{
    public class User : IdentityUser<int>
    {
        public ClientInfo? ClientInfo { get; set; }
        public SellerInfo? SellerInfo { get; set; }
        public AdminInfo? AdminInfo { get; set; }

        private string _role = UserRoles.Client;
        private string _fullName = null!;
        private string _password = null!;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (value.Length < 3)
                {
                    throw new ArgumentException("Full name must be at least 3 characters long");
                }
                if (value.Length > 50)
                {
                    throw new ArgumentException("Full name must be at most 50 characters long");
                }
                _fullName = value;
            }
        }

        [NotMapped]
        public string Username
        {
            get { return UserName ?? string.Empty; }
            set
            {
                if (value.Length < 3)
                {
                    throw new ArgumentException("Username must be at least 3 characters long");
                }
                if (value.Length > 20)
                {
                    throw new ArgumentException("Username must be at most 20 characters long");
                }
                UserName = value;
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (value.Length < 4)
                {
                    throw new ArgumentException("Password must be at least 4 characters long");
                }
                if (value.Length > 32)
                {
                    throw new ArgumentException("Password must be at most 32 characters long");
                }
                _password = value;
            }
        }
        public string Role
        {
            get { return _role; }
            set
            {
                if (value != UserRoles.Admin && value != UserRoles.Seller && value != UserRoles.Client)
                {
                    throw new ArgumentException($"Role must be either '{UserRoles.Admin}', '{UserRoles.Seller}', or '{UserRoles.Client}'");
                }
                _role = value;
            }
        }

        public override string? Email
        {
            get { return base.Email; }
            set
            {
                if (value != null && !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    throw new ArgumentException("Email must be a valid email address");
                }
                base.Email = value;
            }
        }

        public override string? PhoneNumber
        {
            get { return base.PhoneNumber; }
            set
            {
                if (value != null && !Regex.IsMatch(value, @"^\+?[0-9]{7,15}$"))
                {
                    throw new ArgumentException("Phone number must be a valid phone number with 7 to 15 digits, optionally starting with '+'");
                }
                base.PhoneNumber = value;
            }
        }
    }
}

