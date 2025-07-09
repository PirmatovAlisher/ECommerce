using BasketService.API.Models;
using BasketService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class BasketController : ControllerBase
	{
		private readonly IBasketService _basketService;

		public BasketController(IBasketService basketService)
		{
			_basketService = basketService;
		}

		[HttpGet("{userName}")]
		public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
		{
			var basket = await _basketService.GetBasketAsync(userName);
			return Ok(basket);
		}

		[HttpPost]
		public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
		{
			var updatedBasket = await _basketService.UpdateBasketAsync(basket);
			return Ok(updatedBasket);
		}

		[HttpDelete("{userName}")]
		public async Task<IActionResult> DeleteBasket(string userName)
		{
			await _basketService.DeleteBasketAsync(userName);
			return NoContent();
		}

		[HttpPost("{userName}/items")]
		public async Task<ActionResult<ShoppingCart>> AddItem(string userName, [FromBody] ShoppingCartItem item)
		{
			await _basketService.AddItemToBasketAsync(userName, item);
			var updatedBasket = await _basketService.GetBasketAsync(userName);
			return Ok(updatedBasket);
		}

		[HttpDelete("{userName}/items/{productId}")]
		public async Task<ActionResult<ShoppingCart>> RemoveItem(string userName, string productId)
		{
			await _basketService.RemoveItemFromBasketAsync(userName, productId);
			var updatedBasket = await _basketService.GetBasketAsync(userName);
			return Ok(updatedBasket);
		}
	}
}
