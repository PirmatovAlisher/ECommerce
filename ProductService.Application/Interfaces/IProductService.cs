using ProductService.Application.DTOs;

namespace ProductService.Application.Interfaces
{
	public interface IProductService
	{
		Task<ProductDto> GetProductByIdAsync(string id);
		Task<IEnumerable<ProductDto>> GetAllProductsAsync();
		Task<ProductDto> AddProductAsync(CreateProductDto productDto);
		Task UpdateProductAsync(ProductDto productDto);
		Task DeleteProductAsync(string id);
		Task<IEnumerable<ProductDto>> FilterProductsAsync(string name, string description);
	}
}
