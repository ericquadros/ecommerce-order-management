using System.Net;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;

namespace EcommerceOrderManagement.Tests.Integrations.Orders;

public class CreateOrdersIntegrationTests(IntegrationTestsSetup integrationTestsSetup): BaseTest(integrationTestsSetup)
{
    private readonly HttpClient _httpClient = integrationTestsSetup.HttpClient;
    private readonly IntegrationTestsSetup _integrationTestsSetup = integrationTestsSetup;

    [Fact]
    public async Task CreateOrderWithPix_ShouldReturnCreated_WhenOrderIsValid()
    {
        // Arrange
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
        var response = await _httpClient.PostAsync("/orders/", httpContent);
        
        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdOrderContent = await response.Content.ReadAsStringAsync();
        var createdOrder = JsonConvert.DeserializeObject<CreatedOrderResponse>(createdOrderContent);

        createdOrder.Should().NotBeNull();
        createdOrder.Message.Should().Be("Order created successfully!");
        createdOrder.Data.OrderId.Should().NotBeEmpty(); // Verificando se o OrderId foi gerado
        createdOrder.Data.Status.Should().Be("AwaitingProcessing");
        createdOrder.Data.TotalAmount.Should().Be(8279.973m); // Valor retornado no exemplo
    }
    
    [Fact]
    public async Task CreateOrderWithCreditCard_ShouldReturnCreated_WhenOrderIsValid()
    {
        // Arrange
        var createOrderRequest = new
        {
            Customer = new
            {
                FirstName = "Daenerys",
                LastName = "Targaryen",
                Email = "daenerys.targaryen@winterfell.com",
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
                Method = "Card",
                Pix = (object)null,
                Card = new
                {
                    CardNumber = "4111111111111111",
                    CardHolder = "Daenerys Targaryen",
                    ExpirationDate = "12/25",
                    Cvv = "123",
                    Installments = 6
                }
            }
        };

        var jsonContent = JsonConvert.SerializeObject(createOrderRequest);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync("/orders/", httpContent);

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdOrderContent = await response.Content.ReadAsStringAsync();
        var createdOrder = JsonConvert.DeserializeObject<CreatedOrderResponse>(createdOrderContent);

        createdOrder.Should().NotBeNull();
        createdOrder.Message.Should().Be("Order created successfully!");
        createdOrder.Data.OrderId.Should().NotBeEmpty(); // Verificando se o OrderId foi gerado
        createdOrder.Data.Status.Should().Be("AwaitingProcessing");
        createdOrder.Data.TotalAmount.Should().Be(8739.9715M); // Valor total esperado no exemplo
    }
    
    [Fact]
    public async Task CreateOrderWithNonExistentProduct_ShouldReturnErrorBadRequest_WhenProductDoesNotExist()
    {
        // Arrange
        var createOrderRequest = new
        {
            Customer = new
            {
                FirstName = "Tyrion",
                LastName = "Lannister",
                Email = "tyrion.lannister@casterlyrock.com",
                Phone = "123-456-7890"
            },
            Items = new[]
            {
                new
                {
                    ProductId = "92a66631-faa3-43fa-a9c6-e11b32edf4a0", // UUID de um produto inexistente
                    Quantity = 1,
                    Price = 999.99
                }
            },
            TotalAmount = 999.99,
            Payment = new
            {
                Method = "Card",
                Pix = (object)null,
                Card = new
                {
                    CardNumber = "4111111111111111",
                    CardHolder = "Tyrion Lannister",
                    ExpirationDate = "12/25",
                    Cvv = "123",
                    Installments = 1
                }
            }
        };

        var jsonContent = JsonConvert.SerializeObject(createOrderRequest);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync("/orders/", httpContent);

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest); 

        var errorContent = await response.Content.ReadAsStringAsync();
        errorContent.Should().Contain("One or more products does not exist."); 
    }
    
    [Fact]
    public async Task CreateOrderWithoutItems_ShouldReturnErrorBadRequest_WhenNoItemsAreProvided()
    {
        // Arrange
        var createOrderRequest = new
        {
            Customer = new
            {
                FirstName = "Arya",
                LastName = "Stark",
                Email = "arya.stark@winterfell.com",
                Phone = "987-654-3210"
            },
            Items = new object[] { }, // Nenhum item de pedido fornecido
            TotalAmount = 0m, // Valor total será 0
            Payment = new
            {
                Method = "Card",
                Pix = (object)null,
                Card = new
                {
                    CardNumber = "4111111111111111",
                    CardHolder = "Arya Stark",
                    ExpirationDate = "12/25",
                    Cvv = "123",
                    Installments = 1
                }
            }
        };

        var jsonContent = JsonConvert.SerializeObject(createOrderRequest);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync("/orders/", httpContent);

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest); // Verificando se retorna 400

        var errorContent = await response.Content.ReadAsStringAsync();
        errorContent.Should().Contain("At least one item must be provided"); // Verificando se a mensagem de erro é apropriada
    }
}

public record CreatedOrderResponse(string Message, OrderData Data);

public record OrderData(Guid OrderId, string Status, decimal TotalAmount);