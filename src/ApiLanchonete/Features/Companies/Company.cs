public class Company
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public ICollection<Branch> Branches { get; set; } = [];
}