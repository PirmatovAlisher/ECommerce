using BasketService.API.Models;

namespace BasketService.API.Services
{
	public interface IBasketService
	{
		Task<ShoppingCart> GetBasketAsync(string userName);
		Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket);
		Task DeleteBasketAsync(string userName);
		Task AddItemToBasketAsync(string userName, ShoppingCartItem item);
		Task RemoveItemFromBasketAsync(string userName, string productId);
	}
}
