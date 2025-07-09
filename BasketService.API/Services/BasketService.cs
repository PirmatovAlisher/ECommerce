using BasketService.API.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BasketService.API.Services
{
	public class BasketService : IBasketService
	{
		private readonly IDistributedCache _redisCache;

		public BasketService(IDistributedCache redisCache)
		{
			_redisCache = redisCache;
		}

		public async Task<ShoppingCart> GetBasketAsync(string userName)
		{
			var basketData = await _redisCache.GetStringAsync(userName);
			if (string.IsNullOrEmpty(basketData))
			{
				return new ShoppingCart(userName);
			}
			return JsonConvert.DeserializeObject<ShoppingCart>(basketData) ?? new ShoppingCart(userName);
		}

		public async Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket)
		{
			await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
			return await GetBasketAsync(basket.UserName);
		}

		public async Task DeleteBasketAsync(string userName)
		{
			await _redisCache.RemoveAsync(userName);
		}

		public async Task AddItemToBasketAsync(string userName, ShoppingCartItem newItem)
		{
			var basket = await GetBasketAsync(userName);
			var existingItem = basket.Items.FirstOrDefault(item => item.ProductId == newItem.ProductId);

			if (existingItem != null)
			{
				existingItem.Quantity += newItem.Quantity;
			}
			else
			{
				basket.Items.Add(newItem);
			}
			await UpdateBasketAsync(basket);
		}

		public async Task RemoveItemFromBasketAsync(string userName, string productId)
		{
			var basket = await GetBasketAsync(userName);
			basket.Items.RemoveAll(item => item.ProductId == productId);
			await UpdateBasketAsync(basket);
		}
	}
}
