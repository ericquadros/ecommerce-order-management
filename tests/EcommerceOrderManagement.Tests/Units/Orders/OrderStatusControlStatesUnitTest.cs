using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.Strategies;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using FluentAssertions;

namespace EcommerceOrderManagement.Tests;

public class OrderStatusControlStatesUnitTest
{
    [Fact] 
    public void CreateOrder_ShouldInitializeCorrectly_WhenValidInputsAreProvided()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m)
        };

        var totalAmount = 3999.00m;
        
        // Act
        var order = new Order(customer, items, totalAmount);

        // Assert
        order.Customer.Should().NotBeNull();
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(3999.00m);
        order.Status.Should().Be(OrderStatus.AwaitingProcessing);
    }

    [Fact]
    public void CompleteOrder_ShouldReturnFailure_WhenNoPaymentIsProvided()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m)
        };

        var totalAmount = 3999.00m;
        var order = new Order(customer, items, totalAmount);

        // Act
        var result = order.CompleteOrder();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("You need to set the payment.");
    }

    [Fact]
    public void CancelOrder_ShouldSucceed_WhenOrderIsAwaitingProcessing()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m)
        };

        var totalAmount = 3999.00m;
        var order = new Order(customer, items, totalAmount);

        // Act
        var result = order.CancelOrder();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void CancelOrder_ShouldReturnFailure_WhenOrderIsAlreadyProcessed()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m)
        };

        var totalAmount = 3999.00m;
        var order = new Order(customer, items, totalAmount);
        var pixPayment = new PixPayment("pix-eywqeuwiyqe-ewewew-ewew-id", false, order.Id);
        order.SetPayment(pixPayment);
        
        order.ProcessingPayment();
        order.Status.Should().Be(OrderStatus.ProcessingPayment);

        // Act
        var result = order.CancelOrder();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Only orders awaiting processing can be cancelled.");
    }

    [Fact]
    public void CalculateTotalWithDiscount_ShouldApplyDiscountStrategiesCorrectly()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m),
            new OrderItem(Guid.NewGuid(),1, 1999.00m)
        };

        var totalAmount = 5998.00m;
        var order = new Order(customer, items, totalAmount);

        var discountStrategies = new List<IDiscountStrategy>
        {
            new OrderSeasonalDiscountStrategy(),
            new OrderQuantityDiscountStrategy() 
        };
        
        order.AddDiscountStrategies(discountStrategies);

        // Act
        order.CalculateTotalWithDiscount();

        // Assert
        order.TotalAmount.Should().Be(5698.1000m); // 10% de desconto (15) + 10.00 fixo
    }

    [Fact]
    public void CompleteOrder_ShouldReturnSuccess_WhenPixPaymentIsProvided()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m),
        };

        var totalAmount = 3999.00m;
        var order = new Order(customer, items, totalAmount);
        var pixPayment = new PixPayment("pix-eywqeuwiyqe-ewewew-ewew-id", false, order.Id);
        order.SetPayment(pixPayment);

        // Act
        var result = order.CompleteOrder();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.AwaitingProcessing);
    }
    
    [Fact]
    public void SetStatusToPaymentCompleted_ShouldBeInStatusPaymentCompleted()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m)
        };

        var totalAmount = 5998.00m;
        var order = new Order(customer, items, totalAmount);

        var cardPayment = new CardPayment("4111111111111111", "Arya Stark", "12/25", "123", 12, false, order.Id);
        order.SetPayment(cardPayment);

        // Act
        order.CompleteOrder();
        order.ProcessingPayment();
        order.PaymentCompleted();
        
        // Assert
        order.Status.Should().Be(OrderStatus.PaymentCompleted);
    }
    
    [Fact]
    public void ConpleteOrderWithoutPayment_ShouldBeInStatusAwaitingProcessing()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m)
        };

        var totalAmount = 5998.00m;
        var order = new Order(customer, items, totalAmount);

        // Act
        order.CompleteOrder();
        var result = order.ProcessingPayment();
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("You need to set the payment");
        order.Status.Should().Be(OrderStatus.AwaitingProcessing);
    }
    
    [Fact]
    public void SetStatusToPickingOrder_ShouldBeInStatusPickingOrder()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m)
        };

        var totalAmount = 5998.00m;
        var order = new Order(customer, items, totalAmount);

        var cardPayment = new CardPayment("4111111111111111", "Arya Stark", "12/25", "123", 12, false, order.Id);
        order.SetPayment(cardPayment);

        // Act
        order.CompleteOrder();
        order.ProcessingPayment();
        order.PaymentCompleted();
        order.PickingOrder();
        
        // Assert
        order.Status.Should().Be(OrderStatus.PickingOrder);
    }
    
    [Fact]
    public void FinalizeOrder_ShouldBeInStatusCompleted()
    {
        // Arrange
        var customer = new Customer("Arya", "Stark", "arya.stark@winterfell.com", "987-654-3210");
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(),1, 3999.00m)
        };

        var totalAmount = 5998.00m;
        var order = new Order(customer, items, totalAmount);

        var cardPayment = new CardPayment("4111111111111111", "Arya Stark", "12/25", "123", 12, false, order.Id);
        order.SetPayment(cardPayment);

        // Act
        order.CompleteOrder();
        order.ProcessingPayment();
        order.PaymentCompleted();
        order.PickingOrder();
        order.FinalizeOrder();
        
        // Assert
        order.Status.Should().Be(OrderStatus.Completed);
    }
}