using ApiLanchonete.Authentication;

namespace ApiLanchonete.Features.Clients;

public class Client
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Address { get; set; }

    public required string City { get; set; }

    public required string State { get; set; }

    public required string CEP { get; set; }

    public required string Country { get; set; }

    public required string Phone { get; set; }

    // User Relationship
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    // Audit Fields
    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}