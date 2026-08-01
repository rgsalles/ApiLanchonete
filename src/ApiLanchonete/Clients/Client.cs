using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Clients;

public class Client
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    [Required]
    [StringLength(200)]
    public required string Address { get; set; }

    [Required]
    [StringLength(100)]
    public required string City { get; set; }

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public required string State { get; set; }

    [Required]
    [RegularExpression(@"^\d{5}-?\d{3}$",
        ErrorMessage = "CEP inválido.")]
    public required string CEP { get; set; }

    [Required]
    [StringLength(100)]
    public required string Country { get; set; }

    [Required]
    [Phone]
    [StringLength(20)]
    public required string PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}