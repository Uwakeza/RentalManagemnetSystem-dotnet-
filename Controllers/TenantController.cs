using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using RentalManagementSystem.Data;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Controllers
{
    public class TenantController
    {
        private readonly DatabaseConnection db = new DatabaseConnection();

        public List<Tenant> GetAll()
        {
            var list = new List<Tenant>();
            using var conn = db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT t.TenantID, t.UserID, t.PhoneNumber, u.FullName, u.Email
                FROM Tenants t
                INNER JOIN Users u ON t.UserID = u.UserID
                ORDER BY u.FullName", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Tenant
                {
                    TenantID = (int)r["TenantID"],
                    UserID = (int)r["UserID"],
                    PhoneNumber = r["PhoneNumber"] == DBNull.Value ? "" : r["PhoneNumber"].ToString()!,
                    FullName = r["FullName"].ToString() ?? "",
                    Email = r["Email"].ToString() ?? ""
                });
            }
            return list;
        }

        public string AddTenant(Tenant t)
        {
            if (string.IsNullOrWhiteSpace(t.FullName)) return "Full name is required.";
            if (string.IsNullOrWhiteSpace(t.Email)) return "Email is required.";

            using var conn = db.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                using var userCmd = new SqlCommand(@"
                    INSERT INTO Users (FullName, Email, PasswordHash, Role)
                    OUTPUT INSERTED.UserID
                    VALUES (@n, @e, CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'tenant123'), 2), 'Tenant')",
                    conn, tx);
                userCmd.Parameters.AddWithValue("@n", t.FullName.Trim());
                userCmd.Parameters.AddWithValue("@e", t.Email.Trim());
                int newUserId = (int)userCmd.ExecuteScalar();

                using var tenantCmd = new SqlCommand(
                    "INSERT INTO Tenants (UserID, PhoneNumber) VALUES (@u, @p)", conn, tx);
                tenantCmd.Parameters.AddWithValue("@u", newUserId);
                tenantCmd.Parameters.AddWithValue("@p", (object?)t.PhoneNumber ?? DBNull.Value);
                tenantCmd.ExecuteNonQuery();

                tx.Commit();
                return "OK";
            }
            catch (SqlException ex)
            {
                tx.Rollback();
                return ex.Number == 2627 || ex.Number == 2601
                    ? "That email is already used."
                    : "Database error: " + ex.Message;
            }
        }

        public string UpdateTenant(Tenant t)
        {
            if (string.IsNullOrWhiteSpace(t.FullName)) return "Full name is required.";
            using var conn = db.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                using var c1 = new SqlCommand(
                    "UPDATE Users SET FullName=@n, Email=@e WHERE UserID=@u", conn, tx);
                c1.Parameters.AddWithValue("@n", t.FullName.Trim());
                c1.Parameters.AddWithValue("@e", t.Email.Trim());
                c1.Parameters.AddWithValue("@u", t.UserID);
                c1.ExecuteNonQuery();

                using var c2 = new SqlCommand(
                    "UPDATE Tenants SET PhoneNumber=@p WHERE TenantID=@id", conn, tx);
                c2.Parameters.AddWithValue("@p", (object?)t.PhoneNumber ?? DBNull.Value);
                c2.Parameters.AddWithValue("@id", t.TenantID);
                c2.ExecuteNonQuery();

                tx.Commit();
                return "OK";
            }
            catch (SqlException ex)
            {
                tx.Rollback();
                return "Database error: " + ex.Message;
            }
        }

        public string DeleteTenant(int tenantId, int userId)
        {
            using var conn = db.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                using var c1 = new SqlCommand("DELETE FROM Tenants WHERE TenantID=@id", conn, tx);
                c1.Parameters.AddWithValue("@id", tenantId);
                c1.ExecuteNonQuery();

                using var c2 = new SqlCommand("DELETE FROM Users WHERE UserID=@u", conn, tx);
                c2.Parameters.AddWithValue("@u", userId);
                c2.ExecuteNonQuery();

                tx.Commit();
                return "OK";
            }
            catch (SqlException)
            {
                tx.Rollback();
                return "Cannot delete: this tenant has rentals or payments linked to them.";
            }
        }
    }
}