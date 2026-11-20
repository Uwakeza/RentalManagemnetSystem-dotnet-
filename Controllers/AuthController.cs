using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Controllers
{
    public class AuthController
    {
        private static string Hash(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        public User? Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            using var conn = new SqlConnection(
                "Server=(localdb)\\MSSQLLocalDB;Database=RentalManagementDB;Trusted_Connection=True;TrustServerCertificate=True;");
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT UserID, FullName, Email, Role FROM Users WHERE Email=@e AND PasswordHash=@p", conn);
            cmd.Parameters.AddWithValue("@e", email.Trim());
            cmd.Parameters.AddWithValue("@p", Hash(password));
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new User
            {
                UserID = (int)r["UserID"],
                FullName = r["FullName"].ToString() ?? "",
                Email = r["Email"].ToString() ?? "",
                Role = r["Role"].ToString() ?? ""
            };
        }
    }
}