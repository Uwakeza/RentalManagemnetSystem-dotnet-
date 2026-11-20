
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using RentalManagementSystem.Data;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Controllers
{
    public class UserController
    {
        private DatabaseConnection db = new DatabaseConnection();

        public bool RegisterUser(User user)
        {
            using (SqlConnection connection = db.GetConnection())
            {
                string query = @"INSERT INTO Users
                    (FullName, Email, PasswordHash, Role)
                    VALUES (@FullName, @Email, @PasswordHash, @Role)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@FullName", user.FullName);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Role", user.Role);

                connection.Open();

                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}
