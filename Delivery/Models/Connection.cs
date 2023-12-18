using Microsoft.Data.SqlClient;

namespace Delivery.Models
{
    public class Connection
    {
        private static SqlConnection? conn;

        // Создание подключения
        public static SqlConnection GetConnection()
        {
            conn = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=delivery;Trusted_Connection=True;");
            return conn;
        }
    }
}
