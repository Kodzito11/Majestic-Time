using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

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

		public string? Password { get; set; }
        public UserRole Role { get; set; } = UserRole.User;



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

        private void ValidatePasswordLength()
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
		private void ValidatePasswordToUpper(string password)
		{
			// Regular expression to check if there's at least one uppercase letter
			if (!Regex.IsMatch(password, @"[A-Z]"))
			{
				Console.WriteLine("Password must contain at least one uppercase letter.");
			}
			else
			{
				Console.WriteLine("Password is valid.");
			}
		}

	

		public void Validate()
		{
			ValidateUserName();
			ValidatePasswordLength();
			//ValidatePasswordToUpper();

		}

		public virtual string ToString()  //virtual - hvis underliggende klasser skal overskrives med egen tostring
		{
			return $"Id: {Id}, UserName {Username}, Email: {Email}";
		}

	}
}
