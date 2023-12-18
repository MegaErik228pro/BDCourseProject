namespace Delivery.Models;

public class Company
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }

    public Company(int id, string name, string path)
    {
        Id = id;
        Name = name;
        Path = path;
    }
}