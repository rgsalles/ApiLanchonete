using ApiLanchonete.Features.Branches;
using ApiLanchonete.Features.Clients;
using ApiLanchonete.Features.Products;

namespace ApiLanchonete.Features.Companies;

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public ICollection<Branch> Branches { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<Client> Clients { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
