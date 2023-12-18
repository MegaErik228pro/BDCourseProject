namespace Delivery.Models
{
    public class Order
    {
        public int? Id;
        public int? UserId;
        public string? Email;
        public string? Company;
        public string? Date;
        public string? Status;
        public string? Method;
        public float? Price;
        public string? Products;

        public Order(int id, int user, string company, string date, string status, string method, float price, string prod)
        {
            Id = id;
            UserId = user;
            Company = company;
            Date = date;
            Status = status;
            Method = method;
            Price = price;
            Products = prod;
        }
        public Order(int id, int user, string company, string date, string status, string method, float price, string prod, string email)
        {
            Id = id;
            UserId = user;
            Company = company;
            Date = date;
            Status = status;
            Method = method;
            Price = price;
            Products = prod;
            Email = email;
        }
    }
}