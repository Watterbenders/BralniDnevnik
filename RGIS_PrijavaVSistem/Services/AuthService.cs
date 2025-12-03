using System;
using System.Linq;

namespace BookTracker.Domain
{
    public interface IAuthService
    {
        User Register(string username, string password);
        User? Login(string username, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly InMemoryDataStore _db;

        public AuthService(InMemoryDataStore db)
        {
            _db = db;
        }

        public User Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Uporabniško ime in geslo sta obvezna.");

            if (_db.Users.Any(u => u.Username == username))
                throw new InvalidOperationException("Uporabnik že obstaja.");

            var user = new User
            {
                Username = username,
                Password = password,
                DisplayName = username
            };

            _db.Users.Add(user);
            return user;
        }

        public User? Login(string username, string password)
        {
            var user = _db.Users.SingleOrDefault(u => u.Username == username);
            if (user == null) return null;
            if (user.Password != password) return null;
            return user;
        }
    }
}
