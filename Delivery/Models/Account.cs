namespace Delivery.Models
{
    public static class Account
    {
        public static int? Id;
        public static bool? IsAuth = false;
        public static bool? IsAdmin = false;
        public static int? AdminKey;
        public static List<Product>? Cart = new List<Product>();
        public static int? Company;
        public static string? CompanyName;
        public static string? urlSearch;
        public static string? urlAllergen;
        public static List<Company>? Companies = new List<Company>();
        public static List<Category> Categories = new List<Category>();
    }
}