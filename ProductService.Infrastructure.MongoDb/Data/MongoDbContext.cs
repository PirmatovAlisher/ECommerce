using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.MongoDb.Data
{
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;

		public MongoDbContext(IOptions<MongoDbSettings> settings)
		{
			var client = new MongoClient(settings.Value.ConnectionString);
			_database = client.GetDatabase(settings.Value.DatabaseName);
		}

		public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
	}

	public class MongoDbSettings
	{
		public string ConnectionString { get; set; } = string.Empty;
		public string DatabaseName { get; set; } = string.Empty;
		public string CollectionName { get; set; } = string.Empty;
	}
}
