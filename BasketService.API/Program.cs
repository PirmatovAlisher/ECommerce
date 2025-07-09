
using BasketService.API.Models;
using BasketService.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace BasketService.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();


			builder.Services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = builder.Configuration.GetValue<string>("RedisCacheSettings:ConnectionString");
			});

			builder.Services.AddScoped<IBasketService, Services.BasketService>();

			var jwtSettings = builder.Configuration.GetSection("JwtSettings");
			var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = true,
					ValidIssuer = jwtSettings["Issuer"],
					ValidateAudience = true,
					ValidAudience = jwtSettings["Audience"],
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero
				};
			});

			builder.Services.AddAuthorization();


			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			 

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();


			// Seed Basket Data
			using (var scope = app.Services.CreateScope())
			{
				var redisCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

				// Seed data for 'testuser@gmail.com'
				string testUserEmail = "testuser@gmail.com";
				string adminUserEmail = "admin@gmail.com";

				// Check if basket already exists for testuser@gmail.com
				var existingBasketTestUser = redisCache.GetString(testUserEmail);
				if (string.IsNullOrEmpty(existingBasketTestUser))
				{
					Console.WriteLine($"Seeding basket data for {testUserEmail}...");
					var testUserBasket = new ShoppingCart(testUserEmail)
					{
						Items = new List<ShoppingCartItem>
			{
				new ShoppingCartItem { ProductId = "B6253D46-7002-4D18-A7A2-140D9CB44E93", ProductName = "Laptop Pro", Quantity = 1, Price = 1200.00m },
				new ShoppingCartItem { ProductId = "31AC4BB8-3DB1-4295-AEB2-D1020AA8839D", ProductName = "External SSD 1TB", Quantity = 2, Price = 150.00m }
			}
					};
					redisCache.SetString(testUserEmail, JsonConvert.SerializeObject(testUserBasket), new DistributedCacheEntryOptions
					{
						AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) // Basket expires in 7 days
					});
					Console.WriteLine($"Basket data seeded for {testUserEmail}.");
				}
				else
				{
					Console.WriteLine($"Basket data already exists for {testUserEmail}. Skipping seeding.");
				}

				// Check if basket already exists for admin@example.com
				var existingBasketAdminUser = redisCache.GetString(adminUserEmail);
				if (string.IsNullOrEmpty(existingBasketAdminUser))
				{
					Console.WriteLine($"Seeding basket data for {adminUserEmail}...");
					var adminUserBasket = new ShoppingCart(adminUserEmail)
					{
						Items = new List<ShoppingCartItem>
			{
				new ShoppingCartItem { ProductId = "D825C168-5FEE-48BD-B092-AC13ACC0E850", ProductName = "Gaming Mouse", Quantity = 1, Price = 75.00m },
				new ShoppingCartItem { ProductId = "6BEC6FC5-0892-4E3E-A02C-A276BA19EDF1", ProductName = "Webcam HD", Quantity = 1, Price = 60.00m }
			}
					};
					redisCache.SetString(adminUserEmail, JsonConvert.SerializeObject(adminUserBasket), new DistributedCacheEntryOptions
					{
						AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) // Basket expires in 7 days
					});
					Console.WriteLine($"Basket data seeded for {adminUserEmail}.");
				}
				else
				{
					Console.WriteLine($"Basket data already exists for {adminUserEmail}. Skipping seeding.");
				}
			}

			app.Run();
		}
	}
}
