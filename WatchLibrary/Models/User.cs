using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
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

        public string? PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.User;

        //  Hash og gem password ved oprettelse af bruger
        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty");

            ValidatePassword(password); // Kald valideringsmetode
            PasswordHash = Argon2.Hash(password); // Hasher password med Argon2
        }

        // Verificér password ved login
        public bool VerifyPassword(string password)
        {
            return PasswordHash != null && Isopoh.Cryptography.Argon2.Argon2.Verify(PasswordHash, password);
        }

        // Add the missing ValidatePassword method
        private void ValidatePassword(string password)
        {
            ValidatePasswordLength(password);
            ValidatePasswordToUpper(password);
        }

        private void ValidateUserName()
        {
            if (string.IsNullOrWhiteSpace(Username))
                throw new ArgumentNullException(nameof(Username), "Username cannot be empty or whitespace");

            if (Username.Length < 3 || Username.Length > 30)
                throw new ArgumentOutOfRangeException(nameof(Username), "Username must be between 3 and 30 characters");

            // Kun tillad bogstaver, tal, underscore (_) og bindestreg (-)
            if (!Regex.IsMatch(Username, @"^[a-zA-Z0-9_-]+$"))
                throw new ArgumentException("Username can only contain letters, numbers, underscores (_), and hyphens (-)");
        }

        private void ValidatePasswordLength(string password)
        {

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(nameof(password), "Password cannot be null or empty");
            }

            if (password.Length < 12 || password.Length > 64)
            {

                throw new ArgumentOutOfRangeException("Password must contain at least 12 characters and cannot exceed 64 characters");

            }


        }
        private void ValidatePasswordToUpper(string password)
        {
            // Regular expression to check if there's at least one uppercase letter
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                throw new ArgumentException("Password must contain at least one uppercase letter.");
            }
        }



        public void Validate()
        {
            ValidateUserName();
            ValidatePasswordLength(PasswordHash);
            //ValidatePasswordToUpper();

        }

        public virtual string ToString()  //virtual - hvis underliggende klasser skal overskrives med egen tostring
        {
            return $"Id: {Id}, UserName {Username}, Email: {Email}";
        }

    }
}
