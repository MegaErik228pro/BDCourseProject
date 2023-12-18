using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Delivery.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;

namespace Delivery.Controllers;

public class CompanyController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static SqlConnection? conn;
    private static SqlConnection? conn1;
    private SqlCommand? command;
    private SqlCommand? command1;
    public List<Category> Categories = new List<Category>();
    public List<Product>? Products;

    public CompanyController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(int idCompany, string Search, string Allergen)
    {
        Account.Company = idCompany;

            conn1 = Connection.GetConnection();
            conn1.Open();
            command1 = new SqlCommand("GetCompanyName", conn1)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter idparam = new SqlParameter("@id", idCompany);
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




        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("GetCategories", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter param1 = new SqlParameter("@id", idCompany);
        command.Parameters.Add(param1);
        using (SqlDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Products = new List<Product>();

                    int id = int.Parse(reader.GetValue(0).ToString());
                    string name = reader.GetValue(1).ToString();

                    if((Search == null || Search == "") && (Allergen == null || Allergen == ""))
                    {
                        conn1 = Connection.GetConnection();
                        conn1.Open();
                        command1 = new SqlCommand("GetProducts", conn1)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        SqlParameter param2 = new SqlParameter("@id", id);
                        command1.Parameters.Add(param2);
                        using (SqlDataReader reader1 = command1.ExecuteReader())
                        {
                            if (reader1.HasRows)
                            {
                                while (reader1.Read())
                                {
                                    int pid = int.Parse(reader1.GetValue(0).ToString());
                                    string pname = reader1.GetValue(1).ToString();
                                    string ppath = reader1.GetValue(2).ToString();
                                    string pdesc = reader1.GetValue(3).ToString();
                                    float pprice = float.Parse(reader1.GetValue(4).ToString());
                                    int pgram = int.Parse(reader1.GetValue(5).ToString());
                                    string pall = reader1.GetValue(6).ToString();
                                    Products.Add(new Product(pid,pname,ppath,pdesc,pprice,id,pgram,pall));
                                }
                            }
                        }
                        conn1.Close();
                    }
                    else if(Allergen == null || Allergen == "")
                    {
                        conn1 = Connection.GetConnection();
                        conn1.Open();
                        command1 = new SqlCommand("SearchProducts", conn1)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        SqlParameter param2 = new SqlParameter("@id", id);
                        command1.Parameters.Add(param2);
                        SqlParameter sparam = new SqlParameter("@search", Search);
                        command1.Parameters.Add(sparam);
                        using (SqlDataReader reader1 = command1.ExecuteReader())
                        {
                            if (reader1.HasRows)
                            {
                                while (reader1.Read())
                                {
                                    int pid = int.Parse(reader1.GetValue(0).ToString());
                                    string pname = reader1.GetValue(1).ToString();
                                    string ppath = reader1.GetValue(2).ToString();
                                    string pdesc = reader1.GetValue(3).ToString();
                                    float pprice = float.Parse(reader1.GetValue(4).ToString());
                                    int pgram = int.Parse(reader1.GetValue(5).ToString());
                                    string pall = reader1.GetValue(6).ToString();
                                    Products.Add(new Product(pid,pname,ppath,pdesc,pprice,id,pgram,pall));
                                }
                            }
                        }
                        conn1.Close();
                    }
                    else if(Search == null || Search == "")
                    {
                        conn1 = Connection.GetConnection();
                        conn1.Open();
                        command1 = new SqlCommand("SearchAllergens", conn1)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        SqlParameter param2 = new SqlParameter("@id", id);
                        command1.Parameters.Add(param2);
                        SqlParameter sparam = new SqlParameter("@search", Allergen);
                        command1.Parameters.Add(sparam);
                        using (SqlDataReader reader1 = command1.ExecuteReader())
                        {
                            if (reader1.HasRows)
                            {
                                while (reader1.Read())
                                {
                                    int pid = int.Parse(reader1.GetValue(0).ToString());
                                    string pname = reader1.GetValue(1).ToString();
                                    string ppath = reader1.GetValue(2).ToString();
                                    string pdesc = reader1.GetValue(3).ToString();
                                    float pprice = float.Parse(reader1.GetValue(4).ToString());
                                    int pgram = int.Parse(reader1.GetValue(5).ToString());
                                    string pall = reader1.GetValue(6).ToString();
                                    Products.Add(new Product(pid,pname,ppath,pdesc,pprice,id,pgram,pall));
                                }
                            }
                        }
                        conn1.Close();
                    }
                    else
                    {
                        conn1 = Connection.GetConnection();
                        conn1.Open();
                        command1 = new SqlCommand("SearchAandP", conn1)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        SqlParameter param2 = new SqlParameter("@id", id);
                        command1.Parameters.Add(param2);
                        SqlParameter sparam = new SqlParameter("@psearch", Search);
                        command1.Parameters.Add(sparam);
                        SqlParameter aparam = new SqlParameter("@search", Allergen);
                        command1.Parameters.Add(aparam);
                        using (SqlDataReader reader1 = command1.ExecuteReader())
                        {
                            if (reader1.HasRows)
                            {
                                while (reader1.Read())
                                {
                                    int pid = int.Parse(reader1.GetValue(0).ToString());
                                    string pname = reader1.GetValue(1).ToString();
                                    string ppath = reader1.GetValue(2).ToString();
                                    string pdesc = reader1.GetValue(3).ToString();
                                    float pprice = float.Parse(reader1.GetValue(4).ToString());
                                    int pgram = int.Parse(reader1.GetValue(5).ToString());
                                    string pall = reader1.GetValue(6).ToString();
                                    Products.Add(new Product(pid,pname,ppath,pdesc,pprice,id,pgram,pall));
                                }
                            }
                        }
                        conn1.Close();
                    }
                    
                    Categories.Add(new Category(id, name, Products));
                }
            }

        }
        conn.Close();
        return View(Categories);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddToCart(int id)
    {
        if (Account.Cart.Count > 0)
        {
            if (CheckCompany(id) == false) Account.Cart.Clear();
        }

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
                int pgram = int.Parse(reader.GetValue(5).ToString());
                string pall = reader.GetValue(6).ToString();
                Account.Cart.Add(new Product(id,pname,ppath,pdesc,pprice,pid,pgram,pall));
            }
        }
        conn.Close();
        return Redirect("~/Company?idCompany=" + Account.Company.ToString() + "&Search=" + Account.urlSearch + "&Allergen=" + Account.urlAllergen);
    }

    [HttpPost]
    public IActionResult DeleteFromCart(int id)
    {
        Product product = new Product();
        foreach (Product item in Account.Cart)
        {
            if(item.Id == id) product = item;
        }
        Account.Cart.Remove(product);
        return Redirect("~/Company?idCompany=" + Account.Company.ToString() + "&Search=" + Account.urlSearch + "&Allergen=" + Account.urlAllergen);
    }

    [HttpPost]
    public IActionResult Search(string product, string allergen)
    {
        Account.urlSearch = WebUtility.UrlEncode(product);
        Account.urlAllergen = WebUtility.UrlEncode(allergen);
        return Redirect("~/Company?idCompany=" + Account.Company.ToString() + "&Search=" + Account.urlSearch + "&Allergen=" + Account.urlAllergen);
    }

    public ActionResult Details(int id)
    {
        Product product = new Product();
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
                int pgram = int.Parse(reader.GetValue(5).ToString());
                string pall = reader.GetValue(6).ToString();
                product = new Product(id,pname,ppath,pdesc,pprice,pid,pgram,pall);
            }
        }
        conn.Close();
        return PartialView(product);
    }

    public bool CheckCompany(int product)
    {
        conn = Connection.GetConnection();
        conn.Open();
        command = new SqlCommand("CheckCompany", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        SqlParameter prodParam = new SqlParameter("@product", product);
        SqlParameter comParam = new SqlParameter("@company", Account.Cart.ElementAt(0).IdCategory);
        SqlParameter valid = new SqlParameter("@valid", SqlDbType.Bit);
        valid.Direction = ParameterDirection.Output;
        command.Parameters.Add(prodParam);
        command.Parameters.Add(comParam);
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
