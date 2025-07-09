using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductsController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
		{
			var products = await _productService.GetAllProductsAsync();
			return Ok(products);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ProductDto>> GetProduct(string id)
		{
			var product = await _productService.GetProductByIdAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			return Ok(product);
		}

		[HttpPost]
		public async Task<ActionResult<ProductDto>> AddProduct([FromBody] CreateProductDto productDto)
		{
			var newProduct = await _productService.AddProductAsync(productDto);
			return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductDto productDto)
		{
			if (id != productDto.Id)
			{
				return BadRequest("ID mismatch");
			}

			var existingProduct = await _productService.GetProductByIdAsync(id);
			if (existingProduct == null)
			{
				return NotFound();
			}

			await _productService.UpdateProductAsync(productDto);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(string id)
		{
			var existingProduct = await _productService.GetProductByIdAsync(id);
			if (existingProduct == null)
			{
				return NotFound();
			}

			await _productService.DeleteProductAsync(id);
			return NoContent();
		}

		[HttpGet("filter")]
		public async Task<ActionResult<IEnumerable<ProductDto>>> FilterProducts(
			[FromQuery] string? name,
			[FromQuery] string? description)
		{
			var products = await _productService.FilterProductsAsync(name, description);
			return Ok(products);
		}
	}

}
