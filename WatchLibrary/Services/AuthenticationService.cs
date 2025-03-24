using Isopoh.Cryptography.Argon2;
using WatchLibrary.Models;
using WatchLibrary.Repositories;
using System;
using WatchLibrary.Database;

namespace WatchLibrary.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;

        public AuthenticationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User? Authenticate(string email, string password, out string message)
        {
            var user = _userRepository.GetByEmail(email); // Bruger direkte databasekald

            if (user == null)
            {
                message = "Invalid email or password.";
                return null;
            }

            // Tjek om kontoen er låst
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                message = $"Account is locked. Try again at {user.LockoutEnd}";
                return null;
            }

            // 🔑 Verificer adgangskoden med den hashed version
            if (!Argon2.Verify(user.PasswordHash, password)) // Bruger PasswordHash i stedet for Password
            {
                user.FailedAttempts++;

                if (user.FailedAttempts >= 3)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(10);
                    message = "Account locked due to multiple failed login attempts.";
                }
                else
                {
                    message = "Invalid email or password.";
                }

                _userRepository.Update(user); // Opdaterer fejl i databasen
                return null; // Returnerer null i stedet for false
            }

            //  Login succes, nulstil mislykkede loginforsøg KUN hvis de var forskellige fra 0
            if (user.FailedAttempts > 0 || user.LockoutEnd != null)
            {
                user.FailedAttempts = 0;
                user.LockoutEnd = null;
                _userRepository.Update(user);
            }

            message = "Login successful.";
            return user; // ✅ Returnerer User i stedet for bool
        }
    }
}

