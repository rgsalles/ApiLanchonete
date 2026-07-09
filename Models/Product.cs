namespace ApiLanchonete.Models;

public class Product
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableUntil { get; set; }
}