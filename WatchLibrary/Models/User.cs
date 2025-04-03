using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Isopoh.Cryptography.Argon2;

namespace WatchLibrary.Models
{
    public class User
    {

        public enum UserRole
        {
            User = 0,
            Admin = 1,
            Guest = 2
        }


        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }

        public int FailedAttempts { get; set; } = 0; // Hvor mange gange brugeren har tastet forkert
        public DateTime? LockoutEnd { get; set; } = null; // Hvornår kontoen låses op

        [JsonIgnore]
        public string? PasswordHash { get; set; }

       

        public UserRole Role { get; set; } = UserRole.User;
        
        
        public string? Password { get; set; } // Tilføjet for at håndtere rå password


     

        
        private void ValidateUserName()
        {
            if (string.IsNullOrWhiteSpace(Username))
                throw new ArgumentNullException(nameof(Username), "Username cannot be empty or whitespace");

            if (Username.Length < 3 || Username.Length > 30)
                throw new ArgumentOutOfRangeException(nameof(Username), "Username must be between 3 and 30 characters");

            if (!Regex.IsMatch(Username, @"^[a-zA-Z0-9_-]+$"))
                throw new ArgumentException("Username can only contain letters, numbers, underscores (_), and hyphens (-)");
        }

        private void ValidateEmail()// Validate email
        {
            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentNullException(nameof(Email), "Email cannot be empty");

            var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");//Regex for email validation
            if (!emailRegex.IsMatch(Email))
                throw new ArgumentException("Invalid email format");
        }


        public void Validate()
        {
            ValidateUserName();
            ValidateEmail();

            if (string.IsNullOrEmpty(Password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(Password));
            }

            ValidateSetPassword(Password);
        }




        // Validate password
        public void ValidatePassword(string password)
        {
            ValidatePasswordLength(password);
            ValidatePasswordComplexity(password);

        }

        // Validate password length
        private void ValidatePasswordLength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            if (password.Length < 12 || password.Length > 64)
                throw new ArgumentOutOfRangeException(nameof(password), "Password must contain at least 12 characters and cannot exceed 64 characters");
        }

        //Validere Uppercase, Lowercase, Digit, Special Character
        private void ValidatePasswordComplexity(string password)
        {
            if (!Regex.IsMatch(password, @"[A-Z]") || !Regex.IsMatch(password, @"[a-z]"))
                throw new ArgumentException("Password must contain at least one uppercase and one lowercase letter.", nameof(password));
            if (!Regex.IsMatch(password, @"\d"))
                throw new ArgumentException("Password must contain at least one digit.", nameof(password));
            if (!Regex.IsMatch(password, @"[\W_]"))
                throw new ArgumentException("Password must contain at least one special character.", nameof(password));
        }


           // Set and hash password
        public void ValidateSetPassword(string password)
        {
            ValidatePassword(password); // Validate the raw password
            PasswordHash = Argon2.Hash(password); // Hash the password

            if (PasswordHash == null)
                throw new ArgumentNullException(nameof(PasswordHash), "Password cannot be null");

        }

    

        // Verify password
        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(PasswordHash))
                throw new InvalidOperationException("Password hash is not set.");

            return Argon2.Verify(PasswordHash, password);
        }

        // Override ToString for debugging
        public override string ToString()
        {
            return $"Id: {Id}, Username: {Username}, Email: {Email}";
        }
    }
}