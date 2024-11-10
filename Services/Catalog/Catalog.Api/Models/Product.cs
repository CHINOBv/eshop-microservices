namespace Catalog.Api.Models;

public class Product
{
    public Guid id { get; set; }
    public string Name { get; set; } = default!;
    public List<string> Categories { get; set; } = new();
    public string Description { get; set; } = default!;
    public string ImageFile { get; set; } = default!;
    public double Price { get; set; }
}