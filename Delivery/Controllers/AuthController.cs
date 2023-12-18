using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delivery.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Delivery.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static SqlConnection? conn;
    private SqlCommand? command;

    public AuthController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Index(string email, string psw)
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("Auth", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@Username", email);
        SqlParameter param2 = new SqlParameter("@Password", psw);
        SqlParameter isAdmin = new SqlParameter("@isadmin", SqlDbType.Bit);
        SqlParameter id = new SqlParameter("@id", SqlDbType.Int);
        SqlParameter key = new SqlParameter("@key", SqlDbType.Int);
        isAdmin.Direction = ParameterDirection.Output;
        id.Direction = ParameterDirection.Output;
        key.Direction = ParameterDirection.Output;
        command.Parameters.Add(param1);
        command.Parameters.Add(param2);
        command.Parameters.Add(isAdmin);
        command.Parameters.Add(id);
        command.Parameters.Add(key);
        command.ExecuteNonQuery();
        bool? IsAdmin = isAdmin.Value as bool?;
        int? Id = id.Value as int?;
        int? Key = key.Value as int?;
        if(Id != null)
        {
            Account.IsAuth = true;
            Account.Id = Id;
            if(IsAdmin == true)
            {
                Account.IsAdmin = true;
                Account.AdminKey = Key;
            } 
            else Account.IsAdmin = false;
        }
        else
        {
            ViewData["Message"] = "Неверная почта или пароль";
            return View();
        }
        conn.Close();
        return Redirect("~/Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
