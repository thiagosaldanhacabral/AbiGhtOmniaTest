namespace AbiGhtOmniaTest.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Name Name { get; set; } = new();
    public Address Address { get; set; } = new();
    public string Phone { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.Active;
    public UserRole Role { get; set; } = UserRole.Customer;
}

public class Name
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class Address
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public GeoLocation Geolocation { get; set; } = new();
}

public class GeoLocation
{
    public string Lat { get; set; } = string.Empty;
    public string Long { get; set; } = string.Empty;
}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended
}

public enum UserRole
{
    Customer,
    Manager,
    Admin
}