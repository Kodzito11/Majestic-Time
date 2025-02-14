using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchLibrary.Models;
using Microsoft.Data.SqlClient;
using WatchLibrary.Database;

namespace WatchLibrary.Repositories
{
    public class WatchRepository
    {

        private readonly DBConnection _dbConnection;

        public WatchRepository(DBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }




        public List<Watch> GetAll()
        {
            var watches = new List<Watch>();
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("SELECT Id, Brand, Model, ReferenceNumber, Year, Accessories, Functions, Size, Condition, Description, Price FROM Watches", conn);

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var watch = new Watch
                    {
                        Id = reader.GetInt32(0),
                        Brand = reader.GetString(1),
                        Model = reader.GetString(2),
                        ReferenceNumber = reader.GetString(3),
                        Year = reader.GetInt32(4),
                        Accessories = reader.GetString(5),
                        Functions = reader.GetString(6),
                        Size = reader.GetInt32(7),
                        Condition = reader.GetInt32(8),
                        Description = reader.GetString(9),
                        Price = reader.GetDecimal(10)
                    };
                    watches.Add(watch);
                }
            }
            finally
            {
                conn.Close();
            }

            return watches;
        }

        public Watch GetById(int id)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("SELECT Id, Brand, Model, ReferenceNumber, Year, Accessories, Functions, Size, Condition, Description, Price FROM Watches WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Watch
                    {
                        Id = reader.GetInt32(0),
                        Brand = reader.GetString(1),
                        Model = reader.GetString(2),
                        ReferenceNumber = reader.GetString(3),
                        Year = reader.GetInt32(4),
                        Accessories = reader.GetString(5),
                        Functions = reader.GetString(6),
                        Size = reader.GetInt32(7),
                        Condition = reader.GetInt32(8),
                        Description = reader.GetString(9),
                        Price = reader.GetDecimal(10)
                    };
                }
            }
            finally
            {
                conn.Close();
            }

            return null;
        }

        public void Add(Watch watch)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("INSERT INTO Watches (Brand, Model, ReferenceNumber, Year, Accessories, Functions, Size, Condition, Description, Price) VALUES (@Brand, @Model, @ReferenceNumber, @Year, @Accessories, @Functions, @Size, @Condition, @Description, @Price)", conn);

            cmd.Parameters.AddWithValue("@Brand", watch.Brand);
            cmd.Parameters.AddWithValue("@Model", watch.Model);
            cmd.Parameters.AddWithValue("@ReferenceNumber", watch.ReferenceNumber);
            cmd.Parameters.AddWithValue("@Year", watch.Year);
            cmd.Parameters.AddWithValue("@Accessories", watch.Accessories);
            cmd.Parameters.AddWithValue("@Functions", watch.Functions);
            cmd.Parameters.AddWithValue("@Size", watch.Size);
            cmd.Parameters.AddWithValue("@Condition", watch.Condition);
            cmd.Parameters.AddWithValue("@Description", watch.Description);
            cmd.Parameters.AddWithValue("@Price", watch.Price);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        public void Update(Watch watch)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("UPDATE Watches SET Brand = @Brand, Model = @Model, ReferenceNumber = @ReferenceNumber, Year = @Year, Accessories = @Accessories, Functions = @Functions, Size = @Size, Condition = @Condition, Description = @Description, Price = @Price WHERE Id = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", watch.Id);
            cmd.Parameters.AddWithValue("@Brand", watch.Brand);
            cmd.Parameters.AddWithValue("@Model", watch.Model);
            cmd.Parameters.AddWithValue("@ReferenceNumber", watch.ReferenceNumber);
            cmd.Parameters.AddWithValue("@Year", watch.Year);
            cmd.Parameters.AddWithValue("@Accessories", watch.Accessories);
            cmd.Parameters.AddWithValue("@Functions", watch.Functions);
            cmd.Parameters.AddWithValue("@Size", watch.Size);
            cmd.Parameters.AddWithValue("@Condition", watch.Condition);
            cmd.Parameters.AddWithValue("@Description", watch.Description);
            cmd.Parameters.AddWithValue("@Price", watch.Price);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        public void Delete(int id)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("DELETE FROM Watches WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

