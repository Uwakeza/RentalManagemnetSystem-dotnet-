using Microsoft.Data.SqlClient;

namespace RentalManagementSystem.Data
{
    public class DatabaseConnection
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;
              Database=RentalManagementDB;
              Integrated Security=True;
              TrustServerCertificate=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
