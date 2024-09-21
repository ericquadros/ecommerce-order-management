using EcommerceOrderManagement.Domain.Infrastructure;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;

namespace EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;

public class Customer : Entity
{
    public Customer(string firstName, string lastName, Email email, string phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Validate();
    }

    private Customer() // EF
    { }
    
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string Phone { get; private set; }
    
    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
            throw new ArgumentException("First name cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(LastName))
            throw new ArgumentException("Last name cannot be null or empty.");
        // if (string.IsNullOrWhiteSpace(Email))
        //     throw new ArgumentException("Email cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(Phone))
            throw new ArgumentException("Phone cannot be null or empty.");
    }
}

