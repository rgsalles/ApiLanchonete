using System.ComponentModel.DataAnnotations;

namespace ApiLanchonete.Features.Companies;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class CreateCompanyDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(18)]
    public string Cnpj { get; set; } = string.Empty;
}

public class UpdateCompanyDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(18)]
    public string Cnpj { get; set; } = string.Empty;
}
