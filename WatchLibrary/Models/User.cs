using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Isopoh.Cryptography.Argon2;

namespace WatchLibrary.Models
{
    public class User
    {
        public enum UserRole
        {
            Admin,
            User,
        }


        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
       
        [JsonIgnore]
        public string? PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.User;

        public string? Password { get; set; } // Tilføjet for at håndtere rå password


        public void Validate()
        {
            ValidateUserName();
          
        }

        
        private void ValidateUserName()
        {
            if (string.IsNullOrWhiteSpace(Username))
                throw new ArgumentNullException(nameof(Username), "Username cannot be empty or whitespace");

            if (Username.Length < 3 || Username.Length > 30)
                throw new ArgumentOutOfRangeException(nameof(Username), "Username must be between 3 and 30 characters");

            if (!Regex.IsMatch(Username, @"^[a-zA-Z0-9_-]+$"))
                throw new ArgumentException("Username can only contain letters, numbers, underscores (_), and hyphens (-)");
        }

        // Validate password
        public void ValidatePassword(string password)
        {
            ValidatePasswordLength(password);
            ValidatePasswordUppercase(password);
        }

        // Validate password length
        private void ValidatePasswordLength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            if (password.Length < 12 || password.Length > 64)
                throw new ArgumentOutOfRangeException(nameof(password), "Password must contain at least 12 characters and cannot exceed 64 characters");
        }

        // Validate password uppercase
        private void ValidatePasswordUppercase(string password)
        {
            if (!Regex.IsMatch(password, @"[A-Z]"))
                throw new ArgumentException("Password must contain at least one uppercase letter.", nameof(password));
        }

        // Set and hash password
        public void SetPassword(string password)
        {
            ValidatePassword(password); // Validate the raw password
            PasswordHash = Argon2.Hash(password); // Hash the password
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