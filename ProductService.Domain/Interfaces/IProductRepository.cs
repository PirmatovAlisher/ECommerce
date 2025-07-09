using ProductService.Domain.Entities;

namespace ProductService.Domain.Interfaces
{
	public interface IProductRepository
	{
		Task<Product> GetByIdAsync(string id);
		Task<IEnumerable<Product>> GetAllAsync();
		Task AddAsync(Product product);
		Task UpdateAsync(Product product);
		Task DeleteAsync(string id);
		Task<IEnumerable<Product>> FilterAsync(string name, string description);
	}
}
