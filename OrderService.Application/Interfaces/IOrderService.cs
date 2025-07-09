using OrderService.Application.DTOs;

namespace OrderService.Application.Interfaces
{
	public interface IOrderService
	{
		Task<OrderDto> GetOrderByIdAsync(Guid id);
		Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
		Task<OrderDto> AddOrderAsync(CreateOrderDto orderDto);
		Task UpdateOrderAsync(OrderDto orderDto);
		Task DeleteOrderAsync(Guid id);
		Task<IEnumerable<OrderDto>> FilterOrdersAsync(string userName, string status);
	}
}
