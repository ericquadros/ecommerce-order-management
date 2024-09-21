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
                new ProductCategory { Name = "Eletrônicos", Description = "Itens eletrônicos", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProductCategory { Name = "Livros", Description = "Livros e literatura", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProductCategory { Name = "Roupas", Description = "Vestuário e roupas", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProductCategory { Name = "Móveis", Description = "Móveis para casa", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProductCategory { Name = "Brinquedos", Description = "Brinquedos e jogos para crianças", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };

            context.ProductCategories.AddRange(categories);
            context.SaveChanges();
        }

        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product { Id = new Guid("0350ab23-28e1-4bb3-227d-08dcda5a2e50"), Name = "Smartphone Samsung Galaxy S24", Description = "Smartphone Samsung Galaxy S24 5G Preto, 256GB, 8GB de RAM e Câmera Tripla Traseira de até 50MP, Selfie de 12MP", Price = 4099.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Smartphone iPhone 15 Apple (128GB) Preto", Description = "iPhone 15 Apple (128GB) Preto, Tela de 6,1\", 5G e Câmera de 48 MP", Price = 4693.44m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Id = new Guid("bace59ec-d934-4dc3-2394-08dcda78b59d"), Name = "Notebook Gamer Lenovo Loq", Description = "Notebook Gamer Lenovo Loq, Intel Core i5-12450H, 16GB/512GB SSD, RTX2050, Win11, Tela 15,6", Price = 3999.80m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Id = new Guid("8b07897d-6c9c-4e2e-b9dc-08dcda79fcdf"), Name = "Fones de Ouvido", Description = "Fones de ouvido com cancelamento de ruído", Price = 999.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Tablet", Description = "Tablet portátil", Price = 1299.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Leitor de E-Books", Description = "Leitor de livros digitais", Price = 749.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Romance", Description = "Romance best-seller", Price = 29.99m, CategoryId = context.ProductCategories.Skip(1).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Camiseta", Description = "Camiseta de algodão", Price = 59.99m, CategoryId = context.ProductCategories.Skip(2).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Sofá", Description = "Sofá confortável", Price = 999.99m, CategoryId = context.ProductCategories.Skip(3).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Boneco de Ação", Description = "Boneco de ação colecionável", Price = 229.99m, CategoryId = context.ProductCategories.Skip(4).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Jogo de Tabuleiro", Description = "Jogo de tabuleiro popular", Price = 139.99m, CategoryId = context.ProductCategories.Skip(4).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
