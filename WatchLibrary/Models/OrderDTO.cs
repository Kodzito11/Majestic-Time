namespace WatchLibrary.Models
{
	public class OrderDTO
	{
		public string CustomerName { get; set; }
		public string Address { get; set; }
		public List<OrderItemDTO> Items { get; set; }
	}
}
