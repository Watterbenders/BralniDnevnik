using RGIS_PrijavaVSistem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RGIS_PrijavaVSistem.Services
{
    public static class UserStore
    {
        private static readonly List<User> _users = new();

        public static User? Find(string username) =>
            _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        public static User Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Uporabniško ime in geslo sta obvezna.");

            if (Find(username) != null)
                throw new InvalidOperationException("Uporabniško ime že obstaja.");

            var user = new User { Username = username.Trim(), Password = password };
            _users.Add(user);
            return user;
        }

        public static User? Login(string username, string password)
        {
            var user = Find(username);
            if (user == null) return null;
            if (user.Password != password) return null;
            return user;
        }
    }
}
