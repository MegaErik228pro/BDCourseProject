namespace Delivery.Models;

public class Category
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public List<Product> Products = new List<Product>();

    public Category(int id, string name, List<Product> prod)
    {
        Id = id;
        Name = name;
        Products = prod;
    }
    public Category() {}
}