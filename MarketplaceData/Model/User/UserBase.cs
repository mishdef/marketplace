using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using VetClassLibrary.Model.User;

namespace MarketplaceData.Model.User
{
    public abstract class UserBase
    {
        private string _role = UserRoles.Client;
        private string _fullName = null!;
        private string _username = null!;
        private string _password = null!;
        private string _email = null!;
        private string _phoneNumber = null!;

        public int Id { get; set; }
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

        public string Username
        {
            get { return _username; }
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
                _username = value;
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

        public string Email
        {
            get { return _email; }
            set
            {
                if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    throw new ArgumentException("Email must be a valid email address");
                }
                _email = value;
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (!Regex.IsMatch(value, @"^\+?[0-9]{7,15}$"))
                {
                    throw new ArgumentException("Phone number must be a valid phone number with 7 to 15 digits, optionally starting with '+'");
                }
                _phoneNumber = value;
            }
        }
    }
}
