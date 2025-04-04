using WatchLibrary.Models;
using WatchLibrary.Database;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace WatchLibrary.Repositories
{
    public class CartRepository
    {
        private readonly DBConnection _dbConnection;

        public CartRepository(DBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Gem kurven i databasen for en given bruger
        public void SaveCartToDatabase(List<CartItem> cart, int userId)
        {
            var conn = _dbConnection.GetConnection();
            conn.Open();

            var transaction = conn.BeginTransaction();
            try
            {
                // 1. Fjern eksisterende varer i kurven for denne bruger
                var removeCartItemsCmd = new SqlCommand("DELETE FROM CartItems WHERE UserId = @UserId", conn, transaction);
                removeCartItemsCmd.Parameters.AddWithValue("@UserId", userId);
                removeCartItemsCmd.ExecuteNonQuery();

                // 2. Tilføj de nye varer i kurven
                foreach (var item in cart)
                {
                    var insertItemCmd = new SqlCommand(
                        "INSERT INTO CartItems (WatchId, Quantity, TotalPrice, UserId) VALUES (@WatchId, @Quantity, @TotalPrice, @UserId)",
                        conn, transaction);

                    insertItemCmd.Parameters.AddWithValue("@WatchId", item.WatchId);
                    insertItemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    insertItemCmd.Parameters.AddWithValue("@TotalPrice", item.TotalPrice);
                    insertItemCmd.Parameters.AddWithValue("@UserId", userId);

                    insertItemCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Fejl ved gemning af kurv", ex);
            }
            finally
            {
                conn.Close();
            }
        }

        // Hent kurven fra databasen for en specifik bruger
        public List<CartItem> GetCartFromDatabase(int userId)
        {
            var cartItems = new List<CartItem>();
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("SELECT WatchId, Quantity, TotalPrice, UserId FROM CartItems WHERE UserId = @UserId", conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var cartItem = new CartItem
                    {
                        WatchId = reader.GetInt32(0),
                        Quantity = reader.GetInt32(1),
                        TotalPrice = reader.GetDecimal(2),
                        UserId = reader.GetInt32(3)
                    };
                    cartItems.Add(cartItem);
                }
                reader.Close();
                return cartItems;
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved hentning af kurv", ex);
            }
            finally
            {
                conn.Close();
            }
        }

        // Slet en specifik vare fra kurven
        public void DeleteCartItem(int userId, int watchId)
        {
            var conn = _dbConnection.GetConnection();
            var cmd = new SqlCommand("DELETE FROM CartItems WHERE UserId = @UserId AND WatchId = @WatchId", conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@WatchId", watchId);

            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                // Hvis ingen varer blev slettet, kan du kaste en undtagelse
                if (rowsAffected == 0)
                {
                    throw new Exception("Ingen varer blev slettet. Kontroller, at vare og bruger findes i kurven.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Fejl ved sletning af vare fra kurv", ex);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

