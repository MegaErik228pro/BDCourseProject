using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delivery.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace Delivery.Controllers;

public class ProfileController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static SqlConnection? conn;
    private SqlCommand? command;
    public List<string> accInfo;

    public ProfileController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(string err)
    {
        if(err != null && err != "") ViewData["Message"] = err;
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("GetUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", Account.Id);
        command.Parameters.Add(param1);
        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows)
            {
                reader.Read();
                string? name = reader.GetValue(0).ToString();
                string? phone = reader.GetValue(1).ToString();
                string? addr = reader.GetValue(2).ToString();
                string? email = reader.GetValue(3).ToString();
                string? pass = reader.GetValue(4).ToString();
                accInfo = new List<string>{name, phone, addr, email, pass};
            }
        }
        conn.Close();
        return View(accInfo);
    }

    [HttpPost]
    public IActionResult Update(string name, string phone, string adr, string psw)
    {
        if(!Regex.IsMatch(phone, "^((\\+375|375)+([0-9]){9})$")) //+375 12 345 67 89
        {
            return RedirectToAction("Index" , new { err = "Неверный номер телефона" });
        }
        if(!Regex.IsMatch(psw, "(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$"))
        {
            if(psw.Length < 8) return RedirectToAction("Index" , new { err = "Пароль должен содержать минимум 8 символов" });
            return RedirectToAction("Index" , new { err = "Пароль должен содержать как минимум одну цифру, одну заглавную и одну строчную буквы" });
        }
        
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("UpdateUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", Account.Id);
        SqlParameter param2 = new SqlParameter("@name", name);
        SqlParameter param3 = new SqlParameter("@phoneNo", phone);
        SqlParameter param4 = new SqlParameter("@address", adr);
        SqlParameter param5 = new SqlParameter("@password", psw);
        command.Parameters.Add(param1);
        command.Parameters.Add(param2);
        command.Parameters.Add(param3);
        command.Parameters.Add(param4);
        command.Parameters.Add(param5);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Exit()
    {
        Account.IsAuth = false;
        Account.IsAdmin = false;
        Account.AdminKey = null;
        Account.Cart.Clear();
        return Redirect("~/Home");
    }

    [HttpPost]
    public IActionResult Delete()
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("DeleteUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", Account.Id);
        command.Parameters.Add(param1);
        command.ExecuteNonQuery();
        conn.Close();
        Account.IsAuth = false;
        Account.IsAdmin = false;
        Account.AdminKey = null;
        Account.Cart.Clear();
        return Redirect("~/Home");
    }

    public bool CheckEmail(string email)
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("CheckEmail", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter emailParam = new SqlParameter("@email", email);
        SqlParameter valid = new SqlParameter("@valid", SqlDbType.Bit);
        valid.Direction = ParameterDirection.Output;
        command.Parameters.Add(emailParam);
        command.Parameters.Add(valid);
        command.ExecuteNonQuery();
        bool? Valid = valid.Value as bool?;
        conn.Close();
        return (bool)Valid;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
