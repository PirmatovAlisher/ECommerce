using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _productRepository;

		public ProductService(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public async Task<ProductDto> GetProductByIdAsync(string id)
		{
			var product = await _productRepository.GetByIdAsync(id);
			return product == null ? null : MapToDto(product);
		}

		public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
		{
			var products = await _productRepository.GetAllAsync();
			return products.Select(MapToDto);
		}

		public async Task<ProductDto> AddProductAsync(CreateProductDto createProductDto)
		{
			var product = new Product
			{
				Name = createProductDto.Name,
				Description = createProductDto.Description,
				Price = createProductDto.Price,
				Stock = createProductDto.Stock
			};
			await _productRepository.AddAsync(product);
			return MapToDto(product);
		}

		public async Task UpdateProductAsync(ProductDto productDto)
		{
			var product = new Product
			{
				Id = Guid.Parse(productDto.Id),
				Name = productDto.Name,
				Description = productDto.Description,
				Price = productDto.Price,
				Stock = productDto.Stock
			};
			await _productRepository.UpdateAsync(product);
		}

		public async Task DeleteProductAsync(string id)
		{
			await _productRepository.DeleteAsync(id);
		}

		public async Task<IEnumerable<ProductDto>> FilterProductsAsync(string name, string description)
		{
			var products = await _productRepository.FilterAsync(name, description);
			return products.Select(MapToDto);
		}

		private ProductDto MapToDto(Product product)
		{
			return new ProductDto
			{
				Id = product.Id.ToString(),
				Name = product.Name,
				Description = product.Description,
				Price = product.Price,
				Stock = product.Stock
			};
		}
	}
}
