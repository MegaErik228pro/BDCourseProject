using Microsoft.Data.SqlClient;
using System.Data;

namespace Delivery.Models;

public class Product
{
    private static SqlConnection? conn;
    private SqlCommand? command;
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? Description { get; set; }
    public float? Price { get; set; }
    public int? IdCategory { get; set; }
    public int? Gram { get; set; }
    public string? Allergens { get; set; }
    public string? AllergensText = "";
    public int? Count = 1;

    public Product(int id, string name, string path, string desc, float price, int cat, int gram, string allerg)
    {
        Id = id;
        Name = name;
        Path = path;
        Description = desc;
        Price = price;
        IdCategory = cat;
        Gram = gram;
        Allergens = allerg;

        if(allerg == null || allerg == "")
        {
            AllergensText = "Нет";
        }
        else
        {
            foreach(char c in allerg)
            {
                conn = Connection.GetConnection();
                conn.Open();
                command = new SqlCommand("GetAllergen", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlParameter param = new SqlParameter("@id", int.Parse(c.ToString()));
                command.Parameters.Add(param);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string aname = reader.GetValue(0).ToString();
                            AllergensText += aname + " ";
                        }
                    }
                }
                conn.Close();
            }
        }
    }
    public Product() {}
}