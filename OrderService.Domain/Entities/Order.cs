using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
	public class Order
	{
		public Guid Id { get; set; }
		public string UserName { get; set; } = string.Empty;
		public decimal TotalPrice { get; set; }
		public DateTime OrderDate { get; set; } = DateTime.UtcNow;
		public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
	}
}
