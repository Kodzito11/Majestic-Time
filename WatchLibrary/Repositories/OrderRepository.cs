using WatchLibrary.Models;
using WatchLibrary.Database;
using Microsoft.Data.SqlClient;

namespace WatchLibrary.Repositories
{
	public class OrderRepository
	{
		private readonly DBConnection _dbConnection;

		public OrderRepository(DBConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public void SaveOrder(OrderDTO order)
		{
			var conn = _dbConnection.GetConnection();
			conn.Open();

			var transaction = conn.BeginTransaction();
			try
			{
				// 1. Indsæt i Orders
				var insertOrder = new SqlCommand(
					"INSERT INTO Orders (CustomerName, Address) OUTPUT INSERTED.Id VALUES (@CustomerName, @Address)",
					conn, transaction);

				insertOrder.Parameters.AddWithValue("@CustomerName", order.CustomerName);
				insertOrder.Parameters.AddWithValue("@Address", order.Address);

				var orderId = (int)insertOrder.ExecuteScalar();

				// 2. Indsæt tilhørende OrderItems
				foreach (var item in order.Items)
				{
					var insertItem = new SqlCommand(
						"INSERT INTO OrderItems (OrderId, WatchId, Quantity) VALUES (@OrderId, @WatchId, @Quantity)",
						conn, transaction);

					insertItem.Parameters.AddWithValue("@OrderId", orderId);
					insertItem.Parameters.AddWithValue("@WatchId", item.WatchId);
					insertItem.Parameters.AddWithValue("@Quantity", item.Quantity);

					insertItem.ExecuteNonQuery();
				}

				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
			finally
			{
				conn.Close();
			}
		}
	}
}
