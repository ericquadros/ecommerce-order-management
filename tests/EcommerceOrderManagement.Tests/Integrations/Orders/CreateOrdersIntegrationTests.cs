using System.Net;
using System.Text;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using FluentAssertions;
using Newtonsoft.Json;

namespace EcommerceOrderManagement.Tests.Integrations.Orders;

public class CreateOrdersIntegrationTests : IClassFixture<OrderManagementWebApplicationFactory>
{
    private readonly OrderManagementWebApplicationFactory _factory;

    public CreateOrdersIntegrationTests(OrderManagementWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreated_WhenOrderIsValid()
    {
        // Arrange
        var httpClient = _factory.CreateClient();

        var createOrderRequest = new
        {
            Customer = new
            {
                FirstName = "John",
                LastName = "Snow",
                Email = "john.snow@winterfell.com",
                Phone = "123-456-7890"
            },
            Items = new[]
            {
                new
                {
                    ProductId = "0350ab23-28e1-4bb3-227d-08dcda5a2e50",
                    Quantity = 2,
                    Price = 4099.99
                },
                new
                {
                    ProductId = "8b07897d-6c9c-4e2e-b9dc-08dcda79fcdf",
                    Quantity = 1,
                    Price = 999.99
                }
            },
            TotalAmount = 9199.97,
            Payment = new
            {
                Method = "Pix",
                Pix = new
                {
                    TransactionId = "abc-123-pix-trans-id"
                },
                CreditCard = (object)null
            }
        };

        var jsonContent = JsonConvert.SerializeObject(createOrderRequest);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await httpClient.PostAsync("/orders/", httpContent);
        
        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdOrderContent = await response.Content.ReadAsStringAsync();
        var createdOrder = JsonConvert.DeserializeObject<CreatedOrderResponse>(createdOrderContent);

        createdOrder.Should().NotBeNull();
        createdOrder.Customer.FirstName.Should().Be("John");
        createdOrder.TotalAmount.Should().Be(9199.97m);
    }
}

public class CreatedOrderResponse
{
    public string Id { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public CustomerResponse Customer { get; set; }
}

public class CustomerResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}