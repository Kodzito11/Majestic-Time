using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Linq;
using WatchLibrary.Database;
using WatchLibrary.Models;

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
            var cmd = new SqlCommand("SELECT Id, Mail, Password FROM Users", conn);

            try
            {
                conn.Open(); //Starter forbindelsen, så vi kan snakke med databasen.
                var reader = cmd.ExecuteReader(); //Udfører SQL'en og giver os adgang til de data, vi har bedt om.
                while (reader.Read()) //gennemgår hver række
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        Password = reader.GetString(2)
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
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }




        public User GetById(int id)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("SELECT Id, Mail, Password FROM Users WHERE Id = @Id", conn);
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
                        Email = reader.GetString(1),
                        Password = reader.GetString(2)
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
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        public User Add(User user)
        {


            user.Validate();
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("Insert into Users (username, mail, password, role) values (@username, @mail, @password, @role); SELECT SCOPE_IDENTITY()", conn);
            cmd.Parameters.AddWithValue("@username", user.Username);


            cmd.Parameters.AddWithValue("@mail", user.Email);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@role", user.Role.ToString()); // Rolle er enum, så vi gemmer det som en streng

            try
            {
                conn.Open();  // Åbner forbindelsen
                var newId = Convert.ToInt32(cmd.ExecuteScalar());  // Kører kommandoen og får ID'et tilbage
                user.Id = newId;  // Sætter det nye ID på brugerobjektet
                return user;  // Returnerer den tilføjede bruger
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved tilføjelse af bruger", ex);
            }
            finally
            {
                conn.Close();
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
            user.Validate();  // Validerer brugerdata

            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("UPDATE Users SET username = @username, mail = @mail, password = @password, role = @role WHERE Id = @Id", conn);

            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@mail", user.Email);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@role", user.Role.ToString());
            cmd.Parameters.AddWithValue("@Id", user.Id);

            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();  // Kører opdateringskommandoen

                if (rowsAffected > 0)
                {
                    return user;  // Returnerer den opdaterede bruger
                }
                return null;  // Returnerer null, hvis ingen bruger blev opdateret (fx hvis ID ikke findes)
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved opdatering af bruger", ex);
            }
            finally
            {
                conn.Close();  // Lukker forbindelsen
            }
        }
    }
}