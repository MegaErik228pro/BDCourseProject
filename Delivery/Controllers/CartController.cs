using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delivery.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Delivery.Controllers;

public class CartController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static SqlConnection? conn;
    private SqlCommand? command;
    private static SqlConnection? conn1;
    private SqlCommand? command1;
    public List<Company> Companies = new List<Company>();

    public CartController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(int id)
    {
        foreach (Product item in Account.Cart)
        {
            if(item.Id == id)
            {
                int index = Account.Cart.IndexOf(item);
                Account.Cart[index].Count++;
            } 
        }
        return Redirect("~/Cart");
    }

    [HttpPost]
    public IActionResult Rem(int id)
    {
        int index = 0;
        foreach (Product item in Account.Cart)
        {
            if(item.Id == id) 
            {
                index = Account.Cart.IndexOf(item);
                Account.Cart[index].Count--;
            }
        }
        if(Account.Cart[index].Count <= 0)
        {
            Account.Cart.Remove(Account.Cart[index]);
        }
        return Redirect("~/Cart");
    }

    [HttpPost]
    public IActionResult CreateOrder(string method)
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("CreateOrder", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@iduser", Account.Id);
        int? idComp = 0;


            conn1 = Connection.GetConnection();
            conn1.Open();
            command1 = new SqlCommand("GetCompanyByCategory", conn1)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter idparam = new SqlParameter("@id", Account.Cart[0].IdCategory);
            command1.Parameters.Add(idparam);
            using (SqlDataReader reader1 = command1.ExecuteReader())
            {
                if (reader1.HasRows)
                {
                    reader1.Read();
                    idComp = int.Parse(reader1.GetValue(0).ToString());
                }
            }
            conn1.Close();




        SqlParameter param2 = new SqlParameter("@idcompany", idComp);
        if(method == "card") method = "Карта";
        else method = "Наличные";
        SqlParameter param3 = new SqlParameter("@method", method);
        float? price = 0;
        foreach(Product item in Account.Cart)
        {
            price += item.Price * item.Count;
        }
        SqlParameter param4 = new SqlParameter("@price", price);
        string? prod = "";
        foreach(Product item in Account.Cart)
        {
            if(Account.Cart.IndexOf(item) == (Account.Cart.Count - 1))
            {
                if(item.Count > 1) prod += item.Name + " x" + item.Count.ToString();
                else prod += item.Name;
            }
            else
            {
                if(item.Count > 1) prod += item.Name + " x" + item.Count.ToString() + " | ";
                else prod += item.Name + " | ";
            }
        }
        SqlParameter param5 = new SqlParameter("@prod", prod);
        command.Parameters.Add(param1);
        command.Parameters.Add(param2);
        command.Parameters.Add(param3);
        command.Parameters.Add(param4);
        command.Parameters.Add(param5);
        command.ExecuteNonQuery();
        conn.Close();

        Account.Cart.Clear();
        return Redirect("~/Orders");
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
