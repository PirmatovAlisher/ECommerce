using MongoDB.Driver;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.MongoDb.Data;

namespace ProductService.Infrastructure.MongoDb.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly IMongoCollection<Product> _products;

		public ProductRepository(MongoDbContext context)
		{
			_products = context.Products;
		}

		public async Task<Product> GetByIdAsync(string id)
		{
			return await _products.Find(p => p.Id == Guid.Parse(id)).FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<Product>> GetAllAsync()
		{
			return await _products.Find(_ => true).ToListAsync();
		}

		public async Task AddAsync(Product product)
		{
			await _products.InsertOneAsync(product);
		}

		public async Task UpdateAsync(Product product)
		{
			await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
		}

		public async Task DeleteAsync(string id)
		{
			await _products.DeleteOneAsync(p => p.Id == Guid.Parse(id));
		}

		public async Task<IEnumerable<Product>> FilterAsync(string name, string description)
		{
			var filterBuilder = Builders<Product>.Filter;
			var filter = filterBuilder.Empty;

			if (!string.IsNullOrWhiteSpace(name))
			{
				filter &= filterBuilder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(name, "i")); // Case-insensitive regex
			}

			if (!string.IsNullOrWhiteSpace(description))
			{
				filter &= filterBuilder.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(description, "i")); // Case-insensitive regex
			}

			return await _products.Find(filter).ToListAsync();
		}
	}
}
