using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces
{
	public interface IOrderRepository
	{
		Task<Order> GetByIdAsync(Guid id);
		Task<IEnumerable<Order>> GetAllAsync();
		Task AddAsync(Order order);
		Task UpdateAsync(Order order);
		Task DeleteAsync(Guid id);
		Task<IEnumerable<Order>> FilterAsync(string userName, string status);
	}
}
