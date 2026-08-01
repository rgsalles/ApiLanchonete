using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Clients;

public class Client
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string CEP { get; set; }
    public required string Country { get; set; }
    public required string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}