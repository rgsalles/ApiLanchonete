using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Authentication;

public class UserDto
{
    public Guid Id { get; set; }

    public required string Email { get; set; }

    public UserRole Role { get; set; }

    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}

public class UpdateUserDto
{
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }

    public UserRole Role { get; set; }

    public bool Active { get; set; }
}

public class RegisterDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }

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
    public required string Phone { get; set; }
}