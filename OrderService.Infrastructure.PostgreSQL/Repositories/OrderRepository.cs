using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.PostgreSQL.Data;

namespace OrderService.Infrastructure.PostgreSQL.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly OrderDbContext _context;

		public OrderRepository(OrderDbContext context)
		{
			_context = context;
		}

		public async Task<Order> GetByIdAsync(Guid id)
		{
			return await _context.Orders
				.Include(o => o.OrderItems)
				.FirstOrDefaultAsync(o => o.Id == id);
		}

		public async Task<IEnumerable<Order>> GetAllAsync()
		{
			return await _context.Orders
				.Include(o => o.OrderItems)
				.ToListAsync();
		}

		public async Task AddAsync(Order order)
		{
			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Order order)
		{
			_context.Orders.Update(order);
			// Handle OrderItems updates (add/remove/update)
			foreach (var item in order.OrderItems)
			{
				if (item.Id == Guid.Empty) // New item
				{
					_context.Entry(item).State = EntityState.Added;
				}
				else // Existing item
				{
					_context.Entry(item).State = EntityState.Modified;
				}
			}

			// Remove items that are no longer in the order
			var existingItems = await _context.OrderItems
				.Where(oi => oi.OrderId.Equals(order.Id))
				.ToListAsync();
			var itemsToRemove = existingItems.Where(ei => !order.OrderItems.Any(ni => ni.Id == ei.Id)).ToList();
			_context.OrderItems.RemoveRange(itemsToRemove);

			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			var order = await _context.Orders.FindAsync(id);
			if (order != null)
			{
				_context.Orders.Remove(order);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Order>> FilterAsync(string userName, string status)
		{
			var query = _context.Orders.Include(o => o.OrderItems).AsQueryable();

			if (!string.IsNullOrWhiteSpace(userName))
			{
				query = query.Where(o => o.UserName.Contains(userName));
			}

			if (!string.IsNullOrWhiteSpace(status))
			{
				query = query.Where(o => o.Status.Contains(status));
			}

			return await query.ToListAsync();
		}
	}
}
