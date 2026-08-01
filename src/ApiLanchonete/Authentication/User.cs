using ApiLanchonete.Clients;
using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Authentication
{
    public class User
    {
        public Guid Id { get; set; }

        public Client? Client { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public enum UserRole
    {
        Customer = 1,
        Staff = 2,
        Admin = 3
    }
}
        