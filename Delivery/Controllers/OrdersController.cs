using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delivery.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Delivery.Controllers;

public class OrdersController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static SqlConnection? conn;
    private SqlCommand? command;
    public List<Order> Orders = new List<Order>();

    public OrdersController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("GetMyOrders", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", Account.Id);
        command.Parameters.Add(param1);
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
                    Orders.Add(new Order(id, 0, company, date, status, paymentMethod, totalPrice, products));
                }
            }
        }
        conn.Close();
        return View(Orders);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
