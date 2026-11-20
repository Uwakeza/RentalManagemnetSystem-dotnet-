using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using RentalManagementSystem.Data;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Controllers
{
    public class PaymentController
    {
        private readonly DatabaseConnection db = new DatabaseConnection();

        public List<RentalChoice> GetRentalChoices()
        {
            var list = new List<RentalChoice>();
            using var conn = db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT r.RentalID, u.FullName, p.Address
                FROM Rentals r
                INNER JOIN Tenants t ON r.TenantID = t.TenantID
                INNER JOIN Users u ON t.UserID = u.UserID
                INNER JOIN Properties p ON r.PropertyID = p.PropertyID
                ORDER BY u.FullName", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new RentalChoice
                {
                    RentalID = (int)rd["RentalID"],
                    Display = rd["FullName"] + " - " + rd["Address"]
                });
            }
            return list;
        }

        public List<Payment> GetAll()
        {
            var list = new List<Payment>();
            using var conn = db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT pay.PaymentID, pay.RentalID, pay.Amount, pay.PaymentDate, pay.Status,
                       u.FullName, p.Address
                FROM Payments pay
                INNER JOIN Rentals r ON pay.RentalID = r.RentalID
                INNER JOIN Tenants t ON r.TenantID = t.TenantID
                INNER JOIN Users u ON t.UserID = u.UserID
                INNER JOIN Properties p ON r.PropertyID = p.PropertyID
                ORDER BY pay.PaymentDate DESC", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Payment
                {
                    PaymentID = (int)rd["PaymentID"],
                    RentalID = (int)rd["RentalID"],
                    Amount = (decimal)rd["Amount"],
                    PaymentDate = (DateTime)rd["PaymentDate"],
                    Status = rd["Status"] == DBNull.Value ? "" : rd["Status"].ToString()!,
                    RentalInfo = rd["FullName"] + " - " + rd["Address"]
                });
            }
            return list;
        }

        public string Add(Payment p)
        {
            string error = Validate(p);
            if (error != "") return error;
            try
            {
                using var conn = db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand(@"
                    INSERT INTO Payments (RentalID, Amount, PaymentDate, Status)
                    VALUES (@r, @a, @d, @s)", conn);
                Fill(cmd, p);
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException ex) { return "Database error: " + ex.Message; }
        }

        public string Update(Payment p)
        {
            string error = Validate(p);
            if (error != "") return error;
            try
            {
                using var conn = db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand(@"
                    UPDATE Payments
                    SET RentalID=@r, Amount=@a, PaymentDate=@d, Status=@s
                    WHERE PaymentID=@id", conn);
                Fill(cmd, p);
                cmd.Parameters.AddWithValue("@id", p.PaymentID);
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException ex) { return "Database error: " + ex.Message; }
        }

        public string Delete(int paymentId)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand("DELETE FROM Payments WHERE PaymentID=@id", conn);
                cmd.Parameters.AddWithValue("@id", paymentId);
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException ex) { return "Database error: " + ex.Message; }
        }

        private static string Validate(Payment p)
        {
            if (p.RentalID <= 0) return "Please choose a rental.";
            if (p.Amount <= 0) return "Amount must be greater than 0.";
            return "";
        }

        private static void Fill(SqlCommand cmd, Payment p)
        {
            cmd.Parameters.AddWithValue("@r", p.RentalID);
            cmd.Parameters.AddWithValue("@a", p.Amount);
            cmd.Parameters.AddWithValue("@d", p.PaymentDate.Date);
            cmd.Parameters.AddWithValue("@s", p.Status);
        }
    }
}