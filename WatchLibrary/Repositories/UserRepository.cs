using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using WatchLibrary.Database;
using WatchLibrary.Models;
using static WatchLibrary.Models.User;
using System.ComponentModel.Design;
using Isopoh.Cryptography.Argon2;

namespace WatchLibrary.Repositories
{
    public class UserRepository
    {
        private readonly DBConnection _dbConnection;

        public UserRepository(DBConnection dbconnection)
        {
            _dbConnection = dbconnection;
        }

        public List<User> GetAll()
        {
            var users = new List<User>();
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("SELECT Id, Username, Email, Password, UserRole FROM Users", conn);

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3),
                        Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString(4)),

                    };
                    users.Add(user);
                }
                reader.Close();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved hentning af brugere", ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
        //public bool EmailExists(string email)
        //{
        //      var conn = _dbConnection.GetConnection();
        //      var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", conn);
        //      cmd.Parameters.AddWithValue("@Email", email);

        //      try
        //      {
        //        conn.Open();
        //        int count = (int)cmd.ExecuteScalar();
        //        return count > 0;
        //      }
        //   catch (Exception ex)
        //      {

        //       throw new Exception("Email Eksitrere", ex);

        //      }

        //   finally
        //      {

        //       conn.Close();

        //      }
        //}
        public User? GetByEmail(string email)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("SELECT Id, Username, Email, PasswordHash, UserRole, FailedAttempts, LockoutEnd FROM Users WHERE Email = @Email", conn);
            cmd.Parameters.AddWithValue("@Email", email);

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        PasswordHash = reader.GetString(3),
                        Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString(4)),
                        FailedAttempts = reader.GetInt32(5),
                        LockoutEnd = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
                    };
                    reader.Close();
                    return user;
                }
                reader.Close();
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved hentning af bruger", ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }


        public User? GetById(int id)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("SELECT Id, username, Email, Password, UserRole FROM Users WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3),
                        Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString(4)),

                    };
                    reader.Close();
                    return user;
                }
                reader.Close();
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved hentning af bruger", ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }


        public User Add(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(user.Password));
            }

            user.ValidateSetPassword(user.Password); // Hasher password
            user.Validate(); // Validerer brugeren

            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("INSERT INTO Users (username, Email, PasswordHash, UserRole) VALUES (@username, @mail, @password, @userrole); SELECT SCOPE_IDENTITY()", conn);

 
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@mail", user.Email);
            cmd.Parameters.AddWithValue("@password", user.PasswordHash);
            cmd.Parameters.AddWithValue("@UserRole", user.Role.ToString());

            try
            {
                conn.Open();
                var newId = Convert.ToInt32(cmd.ExecuteScalar());
                user.Id = newId;
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved tilføjelse af bruger", ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }





        public User? Remove(int id)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("DELETE FROM Users WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();// ExecuteNonQuery() Når du ændrer data, men ikke skal have data tilbage.

                if (rowsAffected > 0) //Lykkedes at slette
                {
                    return new User { Id = id };  // Retunerer id'et på slettet bruger
                }
                return null;  // Ingen rækker slettet
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved sletning af bruger", ex);
            }
            finally
            {
                conn.Close();
            }
        }

        public User? Update(User user)
        {
            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = "DefaultPassword1!"; // Updated default password to meet complexity requirements
            }

            try
            {
                // Validate the raw password before hashing
                user.ValidatePassword(user.Password);
                user.PasswordHash = Argon2.Hash(user.Password); // Hash the password
                user.Validate();  // Validate user data
            }
            catch (ArgumentException ex)
            {
                throw new Exception("Password validation failed: " + ex.Message, ex);
            }

            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("UPDATE Users SET Username = @username, Email = @mail, PasswordHash = @password, UserRole = @role, FailedAttempts = @FailedAttempts, LockoutEnd = @LockoutEnd WHERE Id = @Id", conn);

            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@mail", user.Email);
            cmd.Parameters.AddWithValue("@password", user.PasswordHash);
            cmd.Parameters.AddWithValue("@role", user.Role.ToString());
            cmd.Parameters.AddWithValue("@FailedAttempts", user.FailedAttempts);
            cmd.Parameters.AddWithValue("@LockoutEnd", user.LockoutEnd.HasValue ? (object)user.LockoutEnd.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", user.Id);

            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();  // Execute update command

                if (rowsAffected > 0)
                {
                    return user;  // Return updated user
                }
                return null;  // Return null if no user was updated (e.g., if ID does not exist)
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user", ex);
            }
            finally
            {
                conn.Close();  // Close connection
            }
        }


  
        public User? AuthenticateUser(string email, string password)
        {
            // Hent brugeren med den givne email
            var user = GetByEmail(email);

            if (user == null)
            {
                throw new Exception("Brugeren findes ikke.");
            }

            // Brug VerifyPassword fra User-klassen til at validere adgangskoden
            if (!user.VerifyPassword(password))
            {
                throw new Exception("Forkert adgangskode.");
            }

            // Returner brugeren, hvis autentificeringen lykkedes
            return user;
        }

        // Kaldes, når en bruger indtaster forkert adgangskode
        public void RegisterFailedLogin(User user)
        {
            // Øg tælleren for mislykkede loginforsøg
            user.FailedAttempts++;

            // Hvis brugeren har fejlet 3 gange eller mere
            if (user.FailedAttempts >= 3)
            {
                // Beregn ventetiden: starter med 10 minutter og fordobles for hvert ekstra forsøg (eksponentielt)
                var lockMinutes = 10 * (int)Math.Pow(2, user.FailedAttempts - 3);

                // Sæt tidspunkt hvor brugeren bliver låst ude
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(lockMinutes);
            }

            // Opdater brugeren i databasen med de nye værdier
            Update(user);
        }

        // Kaldes ved succesfuldt login for at nulstille loginforsøg
        public void ResetLoginAttempts(User user)
        {
            // Nulstil tælleren for mislykkede loginforsøg
            user.FailedAttempts = 0;

            // Fjern eventuel låsning
            user.LockoutEnd = null;

            // Opdater brugeren i databasen
            Update(user);
        }

    }
}