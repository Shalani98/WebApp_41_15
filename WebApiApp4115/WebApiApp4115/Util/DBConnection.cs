using System.Data.SqlClient;

using Microsoft.Extensions.Configuration;

namespace WebApiApp4115.Util
{
    public class DBConnection
    {
        private readonly SqlConnection connection;

        public DBConnection()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Ensure this points to where appsettings.json exists
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            // ✅ Use the correct key from appsettings.json
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            connection = new SqlConnection(connectionString);
        }

        public SqlConnection GetConn() => connection;

        public void ConOpen() => connection.Open();

        public void ConClose()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}
