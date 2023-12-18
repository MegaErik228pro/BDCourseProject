using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delivery.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace Delivery.Controllers;

public class RegController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static SqlConnection? conn;
    private SqlCommand? command;
    List<string> accInfo = new List<string>{"", "", "", "", ""};

    public RegController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() => View(accInfo);

    [HttpPost]
    public IActionResult Index(string name, string phone, string adr, string psw, string email)
    {
        if(name != null)
        {
            accInfo = new List<string>{name, phone, adr, psw, email};
        }

        if(CheckEmail(email) == false)
        {
            ViewData["Message"] = "Данная почта уже зарегестрирована";
            return View(accInfo);
        }

        if(!Regex.IsMatch(phone, "^((\\+375|375)+([0-9]){9})$")) //+375 12 345 67 89
        {
            ViewData["Message"] = "Неверный номер телефона";
            return View(accInfo);
        }
        if(!Regex.IsMatch(psw, "(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$"))
        {
            if(psw.Length < 8) ViewData["Message"] = "Пароль должен содержать минимум 8 символов";
            else ViewData["Message"] = "Пароль должен содержать как минимум одну цифру, одну заглавную и одну строчную буквы";
            return View(accInfo);
        }
        if(!Regex.IsMatch(email, "^[-\\w.]+@([A-z0-9][-A-z0-9]+\\.)+[A-z]{2,4}$"))
        {
            ViewData["Message"] = "Неверная почта";
            return View(accInfo);
        }
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("Reg", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@name", name);
        SqlParameter param2 = new SqlParameter("@phoneNo", phone);
        SqlParameter param3 = new SqlParameter("@address", adr);
        SqlParameter param4 = new SqlParameter("@email", email);
        SqlParameter param5 = new SqlParameter("@password", psw);
        SqlParameter id = new SqlParameter("@id", SqlDbType.Int);
        id.Direction = ParameterDirection.Output;
        command.Parameters.Add(param1);
        command.Parameters.Add(param2);
        command.Parameters.Add(param3);
        command.Parameters.Add(param4);
        command.Parameters.Add(param5);
        command.Parameters.Add(id);
        command.ExecuteNonQuery();
        int? Id = id.Value as int?;
        if(Id != null)
        {
            Account.IsAuth = true;
            Account.Id = Id;
            Account.IsAdmin = false;
        }
        conn.Close();
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
