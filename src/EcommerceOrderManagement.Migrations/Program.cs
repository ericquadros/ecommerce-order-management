using EcommerceOrderManagement.Migrations.Context;
using EcommerceOrderManagement.Migrations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<OrderManagementDbContext>();

            var configuration = services.GetRequiredService<IConfiguration>();
            var shouldSeed = configuration.GetValue<bool>("SEED_DATABASE") ||
                             configuration.GetValue<bool>("SeedData:RunSeed");

            if (shouldSeed)
            {
                Console.WriteLine("Running database seed...");
                SeedDatabase(context);
            }
        }
        
        Console.WriteLine("Migrations running!");
        host.Run();
    }
    
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
            })
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("EcommerceOrderManagementDatabase");
                services.AddDbContext<OrderManagementDbContext>(options => options.UseSqlServer(connectionString));
            });
    
    static void SeedDatabase(OrderManagementDbContext context)
    {
        if (!context.ProductCategories.Any())
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { Name = "Electronics", Description = "Electronic items", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProductCategory { Name = "Books", Description = "Books and literature", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new ProductCategory { Name = "Clothing", Description = "Apparel and clothing items", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new ProductCategory { Name = "Furniture", Description = "Household furniture", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new ProductCategory { Name = "Toys", Description = "Toys and games for children", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  }
            };

            context.ProductCategories.AddRange(categories);
            context.SaveChanges();
        }

        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product { Name = "Smartphone", Description = "Latest model smartphone", Price = 499.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new Product { Name = "Laptop", Description = "High performance laptop", Price = 999.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new Product { Name = "Headphones", Description = "Noise-cancelling headphones", Price = 199.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new Product { Name = "Tablet", Description = "Portable tablet", Price = 299.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new Product { Name = "E-Reader", Description = "E-Book reader", Price = 149.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new Product { Name = "Novel", Description = "Best-selling novel", Price = 9.99m, CategoryId = context.ProductCategories.Skip(1).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new Product { Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, CategoryId = context.ProductCategories.Skip(2).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new Product { Name = "Sofa", Description = "Comfortable sofa", Price = 299.99m, CategoryId = context.ProductCategories.Skip(3).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new Product { Name = "Action Figure", Description = "Collectible action figure", Price = 29.99m, CategoryId = context.ProductCategories.Skip(4).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now  },
                new Product { Name = "Board Game", Description = "Popular board game", Price = 39.99m, CategoryId = context.ProductCategories.Skip(4).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
