using System.Data;
using Microsoft.Data.SqlClient;

namespace Delivery.Models
{
    public class User
    {
        public int? Id;
        public string? Name;
        public string? Phone;
        public string? Address;
        public string? Email;
        public string? Password;
        public bool? IsAdmin;
        public int? AdminKey;
        public string? KeyName;
        private static SqlConnection? conn1;
        private SqlCommand? command1;

        public User(int id, string name, string phone, string address, string email, string password, bool isadmin, int? key)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Address = address;
            Email = email;
            Password = password;
            IsAdmin = isadmin;
            AdminKey = key;
            if(AdminKey != null && AdminKey > 0)
            {
                conn1 = Connection.GetConnection();
                conn1.Open();
                command1 = new SqlCommand("GetCompanyName", conn1)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlParameter idparam = new SqlParameter("@id", key);
                command1.Parameters.Add(idparam);
                using (SqlDataReader reader1 = command1.ExecuteReader())
                {
                    if (reader1.HasRows)
                    {
                        reader1.Read();
                        KeyName = reader1.GetValue(0).ToString();
                    }
                }
                conn1.Close();
            }
        }
    }
}