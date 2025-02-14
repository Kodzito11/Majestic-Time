using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace WatchLibrary.Database
{
    public class DBConnection
    {
        private readonly string _connectionString;

       //Pakker værdien ind: gemmer stringen
        public DBConnection(string connectionString) 
        {
            _connectionString = connectionString;
        }


        
        // ADO.NET-klasse, der opretter en ny forbindelse til SQL-databasen
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}