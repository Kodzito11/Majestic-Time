using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace WatchLibrary
{

    public enum UserRole
    {
        Admin,
        User,

    }
    public class User
    {
        public int Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }  

        public UserRole Role { get; set; } = UserRole.User; // Standard rolle



        public void ValidateUserName()
        {
            if (string.IsNullOrWhiteSpace(Username))
                throw new ArgumentNullException(nameof(Username), "Please insert a valid Username");
        }

        public void ValidatePasswordLength()
        {

            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentException(nameof(Password), "Password cannot be null or empty");
            }

            if (Password.Length < 12 && Password.Length > 64)
            {

                throw new ArgumentOutOfRangeException("Password must contain at least 12 characters and cannot exceed 64 characters");

            }


        }
        public void ValidatePasswordToUpper()
        {
            if (string.IsNullOrEmpty(Password))
                throw new ArgumentNullException(nameof(Password), "Password cannot be null or empty");

            if (!Regex.IsMatch(Password, @"[A-Z]"))
            {
                throw new ArgumentException("Password must contain at least one uppercase letter.");
            }
        }



        public void Validate()
        {
            ValidateUserName();
            ValidatePasswordLength();
            ValidatePasswordToUpper();

        }

        public virtual string ToString()  //virtual - hvis underliggende klasser skal overskrives med egen tostring
        {
            return $"Id: {Id}, UserName {Username}, Email: {Email}";
        }

    }
}
