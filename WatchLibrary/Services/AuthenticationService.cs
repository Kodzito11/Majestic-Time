using Isopoh.Cryptography.Argon2;
using WatchLibrary.Models;
using WatchLibrary.Repositories;

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

            try
            {
                ValidatePasswordComplexity(password);
                _userRepository.Update(user); // Opdaterer fejl i databasen
            }
            catch (ArgumentException ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Password validation failed: {ex.Message}");
                message = "Password validation failed: " + ex.Message;
                return null;
            }

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
        return user; //  Returnerer User i stedet for bool
    }

    // Metode til at ændre adgangskode
    public bool ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var user = _userRepository.GetById(userId); // Hent brugeren fra databasen

        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        // Verificer den gamle adgangskode
        string message;
        var authenticatedUser = Authenticate(user?.Email ?? throw new ArgumentNullException(nameof(user.Email)), oldPassword, out message);
        if (authenticatedUser == null)
        {
            throw new ArgumentException("Old password is incorrect.");
        }

        // Valider den nye adgangskode
        try
        {
            ValidatePasswordComplexity(newPassword); // Tjek om den nye adgangskode opfylder kravene
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Password validation failed: {ex.Message}");
        }

        // Hash den nye adgangskode og opdater brugeren i databasen
        user.PasswordHash = Argon2.Hash(newPassword);
        _userRepository.Update(user); // Opdater brugerens adgangskode i databasen

        return true; // Returner true, hvis ændringen er lykkedes
    }

    // Metode til at validere adgangskodekompleksitet
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
