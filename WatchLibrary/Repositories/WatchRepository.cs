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
            var cmd = new SqlCommand("SELECT Id, Brand, Model, ReferenceNumber, Year, Functions, Size, Description, Price FROM Watches", conn);

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
                        Functions = reader.GetString(5),
                        Size = reader.GetInt32(6),
                        Description = reader.GetString(7),
                        Price = reader.GetDecimal(8)
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

        public Watch? GetById(int id)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("SELECT Id, Brand, Model, ReferenceNumber, Year, Functions, Size, Description, Price FROM Watches WHERE Id = @Id", conn);
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
                        Functions = reader.GetString(5),
                        Size = reader.GetInt32(6),
                        Description = reader.GetString(7),
                        Price = reader.GetDecimal(8)
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
            watch.Validate();
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("INSERT INTO Watches (Brand, Model, ReferenceNumber, Year, Functions, Size, Description, Price) VALUES (@Brand, @Model, @ReferenceNumber, @Year, @Functions, @Size, @Description, @Price)", conn);

            cmd.Parameters.AddWithValue("@Brand", watch.Brand);
            cmd.Parameters.AddWithValue("@Model", watch.Model);
            cmd.Parameters.AddWithValue("@ReferenceNumber", watch.ReferenceNumber);
            cmd.Parameters.AddWithValue("@Year", watch.Year);
            cmd.Parameters.AddWithValue("@Functions", watch.Functions);
            cmd.Parameters.AddWithValue("@Size", watch.Size);
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
            var cmd = new SqlCommand("UPDATE Watches SET Brand = @Brand, Model = @Model, ReferenceNumber = @ReferenceNumber, Year = @Year, Functions = @Functions, Size = @Size, Description = @Description, Price = @Price WHERE Id = @Id", conn);


            cmd.Parameters.AddWithValue("@Id", watch.Id);
            cmd.Parameters.AddWithValue("@Brand", watch.Brand);
            cmd.Parameters.AddWithValue("@Model", watch.Model);
            cmd.Parameters.AddWithValue("@ReferenceNumber", watch.ReferenceNumber);
            cmd.Parameters.AddWithValue("@Year", watch.Year);
            cmd.Parameters.AddWithValue("@Functions", watch.Functions);
            cmd.Parameters.AddWithValue("@Size", watch.Size);
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

		public List<WatchDTO> GetAllAsDTO()
		{
			return GetAll().Select(w => new WatchDTO
			{
				Id = w.Id,
				Brand = w.Brand,
				Model = w.Model,
				Price = w.Price
			}).ToList();
		}
		public List<WatchDTO> Search(string query)
		{
			var watches = new List<WatchDTO>();
			var conn = _dbConnection.GetConnection();
			var cmd = new SqlCommand(
				"SELECT Id, Brand, Model, Price FROM Watches WHERE Brand LIKE @q OR Model LIKE @q", conn);
			cmd.Parameters.AddWithValue("@q", $"%{query}%");

			try
			{
				conn.Open();
				var reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					watches.Add(new WatchDTO
					{
						Id = reader.GetInt32(0),
						Brand = reader.GetString(1),
						Model = reader.GetString(2),
						Price = reader.GetDecimal(3)
					});
				}
			}
			finally
			{
				conn.Close();
			}

			return watches;
		}

	}
}

