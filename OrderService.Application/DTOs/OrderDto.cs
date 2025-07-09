namespace OrderService.Application.DTOs
{
	public class OrderDto
	{
		public string Id { get; set; }
		public string UserName { get; set; } = string.Empty;
		public decimal TotalPrice { get; set; }
		public DateTime OrderDate { get; set; }
		public string Status { get; set; } = string.Empty;
		public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
	}

	public class OrderItemDto
	{
		public string Id { get; set; }
		public string ProductId { get; set; } = string.Empty;
		public string ProductName { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}

}
