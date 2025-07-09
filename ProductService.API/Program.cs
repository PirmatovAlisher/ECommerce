
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.MongoDb.Data;
using ProductService.Infrastructure.MongoDb.Repositories;
using System.Text;

namespace ProductService.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.Configure<MongoDbSettings>(
			builder.Configuration.GetSection("MongoDbSettings"));

			builder.Services.AddSingleton<MongoDbContext>();
			builder.Services.AddScoped<IProductRepository, ProductRepository>();
			builder.Services.AddScoped<IProductService, ProductService.Application.Services.ProductService>();

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

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

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

			// Seed data
			using (var scope = app.Services.CreateScope())
			{
				var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
				var productCollection = mongoDbContext.Products;

				if (productCollection.EstimatedDocumentCount() == 0)
				{
					Console.WriteLine("Seeding product data...");
					var products = new List<Product>
						{
							new Product { Name = "Laptop Pro", Description = "High performance laptop for professionals.", Price = 1200.00m, Stock = 50 },
							new Product { Name = "Gaming Mouse", Description = "Ergonomic gaming mouse with RGB lighting.", Price = 75.00m, Stock = 150 },
							new Product { Name = "Mechanical Keyboard", Description = "Tactile mechanical keyboard with customizable keys.", Price = 120.00m, Stock = 100 },
							new Product { Name = "External SSD 1TB", Description = "Fast and portable 1TB external solid state drive.", Price = 150.00m, Stock = 70 },
							new Product { Name = "Webcam HD", Description = "Full HD webcam for video conferencing.", Price = 60.00m, Stock = 200 }
						};
					productCollection.InsertMany(products);
					Console.WriteLine("Product data seeded.");
				}
			}

			app.Run();
		}
	}
}
