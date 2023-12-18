using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delivery.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace Delivery.Controllers;

public class CompanyAdminController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static SqlConnection? conn;
    private SqlCommand? command;
    private static SqlConnection? conn1;
    private SqlCommand? command1;
    public List<Order> orders = new List<Order>(); 
    public Product product = new Product();
    public Category category = new Category();
    public string? stat;
    
    public CompanyAdminController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index() => View();

    public IActionResult Orders()
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("GetCompanyOrders", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param = new SqlParameter("@id", Account.AdminKey);
        command.Parameters.Add(param);
        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = int.Parse(reader.GetValue(0).ToString());
                    string number = reader.GetValue(1).ToString();
                    string date = reader.GetValue(2).ToString();
                    string status = reader.GetValue(3).ToString();
                    string paymentMethod = reader.GetValue(4).ToString();
                    float totalPrice = float.Parse(reader.GetValue(5).ToString());
                    string products = reader.GetValue(6).ToString();
                    int userId = int.Parse(reader.GetValue(7).ToString());
                    string email = reader.GetValue(8).ToString();
                    orders.Add(new Order(id, userId, number, date, status, paymentMethod, totalPrice, products, email));
                }
            }
        }
        conn.Close();
        return View(orders);
    }

    [HttpPost]
    public IActionResult ChangeStatus(int id, string status)
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("ChangeStatus", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", id);
        if(status == "accept") stat = "Принят";
        if(status == "way") stat = "В пути";
        if(status == "finish") stat = "Доставлен";
        if(status == "cancel") stat = "Отменён";
        SqlParameter param2 = new SqlParameter("@status", stat);
        command.Parameters.Add(param1);
        command.Parameters.Add(param2);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Orders");
    }

    public IActionResult History()
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("GetCompanyHistory", conn)
        {
            CommandType = CommandType.StoredProcedure
        };


            conn1 = Connection.GetConnection();
            conn1.Open();
            command1 = new SqlCommand("GetCompanyName", conn1)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter idparam = new SqlParameter("@id", Account.AdminKey);
            command1.Parameters.Add(idparam);
            using (SqlDataReader reader1 = command1.ExecuteReader())
            {
                if (reader1.HasRows)
                {
                    reader1.Read();
                    Account.CompanyName = reader1.GetValue(0).ToString();
                }
            }
            conn1.Close();


        SqlParameter param1 = new SqlParameter("@name", Account.CompanyName);
        command.Parameters.Add(param1);
        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = int.Parse(reader.GetValue(0).ToString());
                    string email = reader.GetValue(1).ToString();
                    string company = reader.GetValue(2).ToString();
                    string date = reader.GetValue(3).ToString();
                    string status = reader.GetValue(4).ToString();
                    string paymentMethod = reader.GetValue(5).ToString();
                    float totalPrice = float.Parse(reader.GetValue(6).ToString());
                    string products = reader.GetValue(7).ToString();
                    orders.Add(new Order(id, 0, company, date, status, paymentMethod, totalPrice, products, email));
                }
            }
        }
        conn.Close();
        return View(orders);
    }

    [HttpPost]
    public IActionResult DeleteProduct(int id)
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("DeleteProduct", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", id);
        command.Parameters.Add(param1);
        command.ExecuteNonQuery();
        conn.Close();
        return Redirect("~/Company?idCompany=" + Account.Company.ToString() + "&Search=" + Account.urlSearch + "&Allergen=" + Account.urlAllergen);
    }

    [HttpPost]
    public IActionResult Product(int id, string operation)
    {

        int pidA = 0;


        ViewData["Operation"] = operation;
        if(operation == "Изменить")
        {
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("GetProduct", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param = new SqlParameter("@id", id);
            command.Parameters.Add(param);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    string pname = reader.GetValue(0).ToString();
                    string ppath = reader.GetValue(1).ToString();
                    string pdesc = reader.GetValue(2).ToString();
                    float pprice = float.Parse(reader.GetValue(3).ToString());
                    int pid = int.Parse(reader.GetValue(4).ToString());
                    pidA = pid;
                    int pgram = int.Parse(reader.GetValue(5).ToString());
                    string pall = reader.GetValue(6).ToString();
                    product = new Product(id, pname, ppath, pdesc, pprice, pid, pgram, pall);
                }
            }
            conn.Close();
        }


        Account.Categories.Clear();
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("GetCategories", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param1 = new SqlParameter("@id", Account.Company);
            command.Parameters.Add(param1);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id1 = int.Parse(reader.GetValue(0).ToString());
                        string name = reader.GetValue(1).ToString();
                        Account.Categories.Add(new Category(id1, name, new List<Product>()));
                    }
                }
            }
            conn.Close();
            
        return View(product);
    }

    [HttpPost]
    public IActionResult UpdateProduct(int id, string name, string path, string desc, float price, int gram, string allerg, int category)
    {
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("UpdateProduct", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param = new SqlParameter("@id", id);
            SqlParameter param1 = new SqlParameter("@name", name);
            SqlParameter param2 = new SqlParameter("@path", path);
            SqlParameter param3 = new SqlParameter("@desc", desc);
            SqlParameter param4 = new SqlParameter("@price", price);
            SqlParameter param5 = new SqlParameter("@category", category);
            SqlParameter param6 = new SqlParameter("@gram", gram);
            SqlParameter param7;
            if (allerg == null) param7 = new SqlParameter("@allerg", DBNull.Value);
            else param7 = new SqlParameter("@allerg", allerg);
            command.Parameters.Add(param);
            command.Parameters.Add(param1);
            command.Parameters.Add(param2);
            command.Parameters.Add(param3);
            command.Parameters.Add(param4);
            command.Parameters.Add(param5);
            command.Parameters.Add(param6);
            command.Parameters.Add(param7);
            command.ExecuteNonQuery();
            conn.Close();
        return Redirect("~/Company?idCompany=" + Account.Company.ToString() + "&Search=" + Account.urlSearch + "&Allergen=" + Account.urlAllergen);
    }

    [HttpPost]
    public IActionResult CreateProduct(string name, string path, string desc, float price, int gram, string allerg, int category)
    {
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("CreateProduct", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param1 = new SqlParameter("@name", name);
            SqlParameter param2 = new SqlParameter("@path", path);
            SqlParameter param3 = new SqlParameter("@desc", desc);
            SqlParameter param4 = new SqlParameter("@price", price);
            SqlParameter param5 = new SqlParameter("@category", category);
            SqlParameter param6 = new SqlParameter("@gram", gram);
            SqlParameter param7;
            if (allerg == null) param7 = new SqlParameter("@allerg", DBNull.Value);
            else param7 = new SqlParameter("@allerg", allerg);
            command.Parameters.Add(param1);
            command.Parameters.Add(param2);
            command.Parameters.Add(param3);
            command.Parameters.Add(param4);
            command.Parameters.Add(param5);
            command.Parameters.Add(param6);
            command.Parameters.Add(param7);
            command.ExecuteNonQuery();
            conn.Close();
        return Redirect("~/Company?idCompany=" + Account.Company.ToString() + "&Search=" + Account.urlSearch + "&Allergen=" + Account.urlAllergen);
    }

    [HttpPost]
    public IActionResult DeleteCategory(int id)
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("DeleteCategory", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", id);
        command.Parameters.Add(param1);
        command.ExecuteNonQuery();
        conn.Close();
        return Redirect("~/Company?idCompany=" + Account.Company.ToString() + "&Search=" + Account.urlSearch + "&Allergen=" + Account.urlAllergen);
    }

    [HttpPost]
    public IActionResult Category(int id, string operation)
    {
        ViewData["Operation"] = operation;
        if(operation == "Изменить")
        {
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("GetCategory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param = new SqlParameter("@id", id);
            command.Parameters.Add(param);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    string name = reader.GetValue(0).ToString();
                    category = new Category(id, name, new List<Product>());
                }
            }
            conn.Close();
        }        
        return View(category);
    }

    [HttpPost]
    public IActionResult UpdateCategory(int id, string name)
    {
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("UpdateCategory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param = new SqlParameter("@id", id);
            SqlParameter param1 = new SqlParameter("@name", name);
            command.Parameters.Add(param);
            command.Parameters.Add(param1);
            command.ExecuteNonQuery();
            conn.Close();
        return Redirect("~/Company?idCompany=" + Account.Company.ToString() + "&Search=" + Account.urlSearch + "&Allergen=" + Account.urlAllergen);
    }

    [HttpPost]
    public IActionResult CreateCategory(string name)
    {
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("CreateCategory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param1 = new SqlParameter("@name", name);
            SqlParameter param2 = new SqlParameter("@company", Account.AdminKey);
            command.Parameters.Add(param1);
            command.Parameters.Add(param2);
            command.ExecuteNonQuery();
            conn.Close();
        return Redirect("~/Company?idCompany=" + Account.Company.ToString() + "&Search=" + Account.urlSearch + "&Allergen=" + Account.urlAllergen);
    }

    [HttpPost]
    public IActionResult ClearOrders()
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("ClearOrdersCompany", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", Account.AdminKey);
        command.Parameters.Add(param1);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Orders");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
