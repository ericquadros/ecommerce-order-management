using System.Globalization;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.OrderManagementContext.Endpoints;
using FastEndpoints;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File("ecommerceOrderManagementLogs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddFastEndpoints(o =>
{
    o.Assemblies = new[]
    {
        typeof(CreateOrderEndpoint).Assembly
    };
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var environment = builder.Environment.EnvironmentName;
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var culture = Environment.GetEnvironmentVariable("ASPNETCORE_CULTURE") ?? "pt-BR";
    var supportedCultures = new[]
    {
        new CultureInfo("pt-BR"),
        new CultureInfo("en-US")
    };

    options.DefaultRequestCulture = new RequestCulture(culture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var connectionString = GetConnectionString(builder);
builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IDbContextFactory<OrderManagementDbContext>, OrderManagementDbContextFactory>();
builder.Services.AddScoped<IEfDbContextAccessor<OrderManagementDbContext>, OrderManagementContextAccessor>();

builder.Services.ConfigureDomainDependenciesServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseFastEndpoints();

// app.UseAuthorization();
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

string? GetConnectionString(WebApplicationBuilder webApplicationBuilder)
{
    return Environment.GetEnvironmentVariable("EcommerceOrderMmanagementDatabase") 
           ?? webApplicationBuilder.Configuration.GetConnectionString("EcommerceOrderMmanagementDatabase");
}

public partial class WebApiProgram
{
}
