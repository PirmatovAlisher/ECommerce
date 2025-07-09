
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.PostgreSQL.Data;
using OrderService.Infrastructure.PostgreSQL.Repositories;
using System.Text;

namespace OrderService.API
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


			builder.Services.AddDbContext<OrderDbContext>(options =>
			options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));

			builder.Services.AddScoped<IOrderRepository, OrderRepository>();
			builder.Services.AddScoped<IOrderService, OrderService.Application.Services.OrderService>(); // Resolve ambiguity

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

			using (var scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
				dbContext.Database.Migrate();

				// Seed data
				if (!dbContext.Orders.Any())
				{
					Console.WriteLine("Seeding order data...");
					var orders = new List<Order>
		{
			new Order
			{
				UserName = "testuser@example.com",
				TotalPrice = 1275.00m,
				OrderDate = DateTime.UtcNow.AddDays(-5),
				Status = "Completed",
				OrderItems = new List<OrderItem>
				{
					new OrderItem { ProductId = Guid.Parse("2E5575EB-3A47-4525-BAE2-6EAC0D6B3BC2"), ProductName = "Cup", Quantity = 1, Price = 49.99m },
					new OrderItem { ProductId = Guid.Parse("A1507259-3EDA-4147-9F31-7AE4A81DFAEB"), ProductName = "Earphone 2", Quantity = 1, Price = 129.95m }
				}
			},
			new Order
			{
				UserName = "anotheruser@example.com",
				TotalPrice = 120.00m,
				OrderDate = DateTime.UtcNow.AddDays(-2),
				Status = "Pending",
				OrderItems = new List<OrderItem>
				{
					new OrderItem { ProductId = Guid.Parse("4530478D-CDCE-4DB9-957B-C42F662CC067"), ProductName = "Electric grass cutter", Quantity = 1, Price = 142.00m }
				}
			}
		};
					dbContext.Orders.AddRange(orders);
					dbContext.SaveChanges();
					Console.WriteLine("Order data seeded.");
				}
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			 

			app.UseAuthentication(); 
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
