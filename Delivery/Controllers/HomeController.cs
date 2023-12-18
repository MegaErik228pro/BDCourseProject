using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delivery.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;

namespace Delivery.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static SqlConnection? conn;
    private SqlCommand? command;
    public List<Company> Companies = new List<Company>();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(string Search)
    {
        if(Search == null || Search == "")
        {
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
                        Companies.Add(new Company(id, name, path));
                    }
                }
            }
            conn.Close();
        }
        else
        {
            conn = Connection.GetConnection();
            conn.Open();
            command = new SqlCommand("SearchCompanies", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter param = new SqlParameter("@search", Search);
            command.Parameters.Add(param);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = int.Parse(reader.GetValue(0).ToString());
                        string name = reader.GetValue(1).ToString();
                        string path = reader.GetValue(2).ToString();
                        Companies.Add(new Company(id, name, path));
                    }
                }
            }
            conn.Close();
        }
        return View(Companies);
    }

    [HttpPost]
    public IActionResult Search(string company)
    {
        string urlEncodedValue = WebUtility.UrlEncode(company);
        return Redirect("~/Home?Search=" + urlEncodedValue);
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
