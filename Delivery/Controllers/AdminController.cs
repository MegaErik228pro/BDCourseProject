using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delivery.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Delivery.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static SqlConnection? conn;
    private SqlCommand? command;
    public Company company = new Company(0, "Test", "Test");
    public List<Order> orders = new List<Order>();
    public List<User> users = new List<User>();
    

    public AdminController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Company(int idCompany, string operation)
    {
        ViewData["Operation"] = operation;
        if(operation == "Изменить")
        {
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("GetCompany", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param = new SqlParameter("@id", idCompany);
            command.Parameters.Add(param);
            using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        int id = int.Parse(reader.GetValue(0).ToString());
                        string name = reader.GetValue(1).ToString();
                        string path = reader.GetValue(2).ToString();
                        company = new Company(id, name, path);
                    }
                }
            conn.Close();
        }
        return View(company);
    }

    [HttpPost]
    public IActionResult Update(int idCompany, string name, string path)
    {
        conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("UpdateCompany", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param1 = new SqlParameter("@id", idCompany);
            SqlParameter param2 = new SqlParameter("@name", name);
            SqlParameter param3 = new SqlParameter("@path", path);
            command.Parameters.Add(param1);
            command.Parameters.Add(param2);
            command.Parameters.Add(param3);
            command.ExecuteNonQuery();
            conn.Close();
        return Redirect("~/Home");
    }

    [HttpPost]
    public IActionResult Create(string name, string path)
    {
        conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("CreateCompany", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param2 = new SqlParameter("@name", name);
            SqlParameter param3 = new SqlParameter("@path", path);
            command.Parameters.Add(param2);
            command.Parameters.Add(param3);
            command.ExecuteNonQuery();
            conn.Close();
        return Redirect("~/Home");
    }

    [HttpPost]
    public IActionResult DeleteCompany(int idCompany)
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("DeleteCompany", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param = new SqlParameter("@id", idCompany);
        command.Parameters.Add(param);
        command.ExecuteNonQuery();
        conn.Close();
        return Redirect("~/Home");
    }

    public IActionResult Orders()
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("GetOrders", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = int.Parse(reader.GetValue(0).ToString());
                    string company = reader.GetValue(1).ToString();
                    string date = reader.GetValue(2).ToString();
                    string status = reader.GetValue(3).ToString();
                    string paymentMethod = reader.GetValue(4).ToString();
                    float totalPrice = float.Parse(reader.GetValue(5).ToString());
                    string products = reader.GetValue(6).ToString();
                    int userId = int.Parse(reader.GetValue(7).ToString());
                    string email = reader.GetValue(8).ToString();
                    orders.Add(new Order(id, userId, company, date, status, paymentMethod, totalPrice, products, email));
                }
            }
        }
        conn.Close();
        return View(orders);
    }

    [HttpPost]
    public IActionResult ClearOrders()
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("ClearOrders", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Orders");
    }

    public IActionResult Accounts()
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("GetUsers", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = int.Parse(reader.GetValue(0).ToString());
                    string name = reader.GetValue(1).ToString();
                    string phone = reader.GetValue(2).ToString();
                    string addr = reader.GetValue(3).ToString();
                    string email = reader.GetValue(4).ToString();
                    string pass = reader.GetValue(5).ToString();
                    bool isAdmin = bool.Parse(reader.GetValue(6).ToString());
                    int? key = -1;
                    if (isAdmin == true) key = int.Parse(reader.GetValue(7).ToString());
                    users.Add(new User(id, name, phone, addr, email, pass, isAdmin, key));
                }
            }
        }
        conn.Close();
            Account.Companies.Clear();
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("GetCompanies", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = int.Parse(reader.GetValue(0).ToString());
                        string name = reader.GetValue(1).ToString();
                        string path = reader.GetValue(2).ToString();
                        Account.Companies.Add(new Company(id, name, path));
                    }
                }
            }
            conn.Close();


        return View(users);
    }

    [HttpPost]
    public IActionResult DeleteUser(int id)
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("DeleteUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", id);
        command.Parameters.Add(param1);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Accounts");
    }

    [HttpPost]
    public IActionResult UpdateRights(int id, string rights, string company)
    {
        bool admin;
        int? k;
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("UpdateRights", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", id);
        if (rights == "admin")
        {
            admin = true;
            if(company == null) k = 0;
            else k = int.Parse(company);
        }
        else
        {
            admin = false;
            k = -1;
        }
        SqlParameter param2 = new SqlParameter("@isadmin", admin);
        SqlParameter param3 = new SqlParameter("@key", k);
        command.Parameters.Add(param1);
        command.Parameters.Add(param2);
        command.Parameters.Add(param3);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Accounts");
    }

    public IActionResult History()
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("GetHistory", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
