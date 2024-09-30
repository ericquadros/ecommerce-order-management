using Dapper;
using EcommerceOrderManagement.Migrations.Context;
using EcommerceOrderManagement.Migrations.Models;
using Microsoft.Data.SqlClient;

namespace EcommerceOrderManagement.Tests.Integrations.Setups;

public static class DatabaseHelper
{
    public async static Task GeraDadosInicializacao<T>(OrderManagementMigrationsDbContext _dbContext, T data) where T : class
    {
        await _dbContext.Set<T>().AddAsync(data);
        await _dbContext.SaveChangesAsync();
    }
    
    public static async Task ExecuteCommand(string command, DynamicParameters? parameters = null)
    {
        using SqlConnection conexao = new(IntegrationTestsSetup.Database.ConnectionString);
        await conexao.ExecuteAsync(command);
    }
    
    public async static Task SeedInitialDatabase(OrderManagementMigrationsDbContext context)
    {
        if (!context.ProductCategories.Any())
        {
            var categories = new List<ProductCategoryModel>
            {
                new ProductCategoryModel { Name = "Eletrônicos", Description = "Itens eletrônicos", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProductCategoryModel { Name = "Livros", Description = "Livros e literatura", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProductCategoryModel { Name = "Roupas", Description = "Vestuário e roupas", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProductCategoryModel { Name = "Móveis", Description = "Móveis para casa", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new ProductCategoryModel { Name = "Brinquedos", Description = "Brinquedos e jogos para crianças", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };

            context.ProductCategories.AddRange(categories);
            context.SaveChanges();
        }

        var a = context.Products.ToList();

        if (!context.Products.Any())
        {
              var products = new List<ProductModel>
              {
                new ProductModel { Id = new Guid("0350ab23-28e1-4bb3-227d-08dcda5a2e50"), Name = "Smartphone Samsung Galaxy S24", Description = "Smartphone Samsung Galaxy S24 5G Preto, 256GB, 8GB de RAM e Câmera Tripla Traseira de até 50MP, Selfie de 12MP", Price = 4099.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 50, OwnerName = "Carlos Silva", OwnerEmail = "carlos.silva@example.com" },
                new ProductModel { Name = "Smartphone iPhone 15 Apple (128GB) Preto", Description = "iPhone 15 Apple (128GB) Preto, Tela de 6,1\", 5G e Câmera de 48 MP", Price = 4693.44m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 75, OwnerName = "Ana Pereira", OwnerEmail = "ana.pereira@example.com" },
                new ProductModel { Id = new Guid("bace59ec-d934-4dc3-2394-08dcda78b59d"), Name = "Notebook Gamer Lenovo Loq", Description = "Notebook Gamer Lenovo Loq, Intel Core i5-12450H, 16GB/512GB SSD, RTX2050, Win11, Tela 15,6", Price = 3999.80m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 30, OwnerName = "Roberto Costa", OwnerEmail = "roberto.costa@example.com" },
                new ProductModel { Id = new Guid("8b07897d-6c9c-4e2e-b9dc-08dcda79fcdf"), Name = "Fones de Ouvido", Description = "Fones de ouvido com cancelamento de ruído", Price = 999.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 20, OwnerName = "Juliana Lima", OwnerEmail = "juliana.lima@example.com" },
                new ProductModel { Name = "Tablet", Description = "Tablet portátil", Price = 1299.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 90, OwnerName = "Fernando Oliveira", OwnerEmail = "fernando.oliveira@example.com" },
                new ProductModel { Name = "Leitor de E-Books", Description = "Leitor de livros digitais", Price = 749.99m, CategoryId = context.ProductCategories.First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 10, OwnerName = "Mariana Santos", OwnerEmail = "mariana.santos@example.com" },
                new ProductModel { Name = "Romance", Description = "Romance best-seller", Price = 29.99m, CategoryId = context.ProductCategories.Skip(1).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 60, OwnerName = "Lucia Almeida", OwnerEmail = "lucia.almeida@example.com" },
                new ProductModel { Name = "Camiseta", Description = "Camiseta de algodão", Price = 59.99m, CategoryId = context.ProductCategories.Skip(2).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 100, OwnerName = "Thiago Martins", OwnerEmail = "thiago.martins@example.com" },
                new ProductModel { Name = "Sofá", Description = "Sofá confortável", Price = 999.99m, CategoryId = context.ProductCategories.Skip(3).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 40, OwnerName = "Sofia Rocha", OwnerEmail = "sofia.rocha@example.com" },
                new ProductModel { Name = "Boneco de Ação", Description = "Boneco de ação colecionável", Price = 229.99m, CategoryId = context.ProductCategories.Skip(4).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 15, OwnerName = "Gustavo Dias", OwnerEmail = "gustavo.dias@example.com" },
                new ProductModel { Name = "Jogo de Tabuleiro", Description = "Jogo de tabuleiro popular", Price = 139.99m, CategoryId = context.ProductCategories.Skip(4).First().Id, ImageUrl = "https://via.placeholder.com/150", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, StockQuantity = 55, OwnerName = "Isabela Ferreira", OwnerEmail = "isabela.ferreira@example.com" }
              };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}