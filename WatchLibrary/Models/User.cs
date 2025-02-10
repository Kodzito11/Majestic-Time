using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace WatchLibrary.Models
{
	public class User
	{
		public int Id { get; set; }

		public string? Username { get; set; }

		public string? Email { get; set; }

		public string? Password { get; set; }



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
		public void ValidatePasswordToUpper(string password)
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

		public void ValidateUsername()
		{
			if (Username.Length > 10)
			{
				throw new ArgumentOutOfRangeException("Please insert a valid Username");
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
