using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs
{
	public class CreateOrderDto
	{
		public string UserName { get; set; } = string.Empty;
		public decimal TotalPrice { get; set; }
		public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
	}

	public class CreateOrderItemDto
	{
		public string ProductId { get; set; } = string.Empty;
		public string ProductName { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
}
