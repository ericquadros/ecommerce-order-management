namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;

public class Email : IEquatable<Email>
{
    public string Address { get; private set; }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address) || !IsValidEmail(address))
        {
            throw new ArgumentException("Invalid email address.");
        }

        Address = address;
    }

    private Email() // EF
    { }

    private bool IsValidEmail(string email)
    {
        // Adicione aqui a lógica de validação de e-mail que desejar
        return email.Contains("@") && email.Contains(".");
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Email);
    }

    public bool Equals(Email other)
    {
        return other != null && Address == other.Address;
    }

    public override int GetHashCode()
    {
        return Address.GetHashCode();
    }
    
    public override string ToString() => Address;

    public static implicit operator string(Email email) => email.Address;
    public static implicit operator Email(string email) => new Email(email);
}