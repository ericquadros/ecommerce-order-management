using System.Globalization;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.OrderManagementContext.Endpoints;
using FastEndpoints;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints(o =>
{
    o.Assemblies = new[]
    {
        typeof(CreateOrderEndpoint).Assembly
        // typeof(CancelOrderEndpoint).Assembly,
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var environment = builder.Environment.EnvironmentName;
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("pt-BR"),
        new CultureInfo("en-US")
    };

    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var connectionString = builder.Configuration.GetConnectionString("OrderManagementDatabase");
builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IEfDbContextAccessor<OrderManagementDbContext>, OrderManagementContextAccessor>();
builder.Services.AddScoped<IDbContextFactory, OrderManagementDbContextFactory>();

builder.Services.ConfigureDomainDependenciesServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// app.UseAuthorization();

app.UseFastEndpoints();

// AddEndpointCulture(app);
// KafkaTestCommunication.Execute("web-api");

app.Run();

void AddEndpointCulture(WebApplication webApplication)
{
    webApplication.MapGet("/culture", () => 
    {
        return new 
        {
            CurrentCulture = CultureInfo.CurrentCulture.Name,
            CurrentUICulture = CultureInfo.CurrentUICulture.Name
        };
    });
}