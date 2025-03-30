using Isopoh.Cryptography.Argon2;
using WatchLibrary.Models;
using WatchLibrary.Repositories;
using System;

public class AuthenticationService
{
    private readonly UserRepository _userRepository;

    public AuthenticationService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User? Authenticate(string email, string password, out string message)
    {
        var user = _userRepository.GetByEmail(email);

        if (user == null)
        {
            message = "Invalid email or password.";
            return null;
        }

        // Check if the account is locked
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            message = $"Account is locked. Try again at {user.LockoutEnd}";
            return null;
        }

        // Verify the password with the hashed version
        if (!Argon2.Verify(user.PasswordHash, password))
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

            try
            {
                _userRepository.Update(user);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error updating user: {ex.Message}");
                message = "Error updating user: " + ex.Message;
                return null;
            }

            return null;
        }

        // Reset failed login attempts if they were different from 0
        if (user.FailedAttempts > 0 || user.LockoutEnd != null)
        {
            user.FailedAttempts = 0;
            user.LockoutEnd = null;
            _userRepository.Update(user);
        }

        message = "Login successful.";
        return user;
    }

    public void ValidatePasswordComplexity(string password)
    {
        if (password.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long.");
        }
        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter.");
        }
        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter.");
        }
        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException("Password must contain at least one digit.");
        }
        if (!password.Any(ch => "!@#$%^&*()_+-=[]{}|;':\",./<>?".Contains(ch)))
        {
            throw new ArgumentException("Password must contain at least one special character.");
        }
    }
}
