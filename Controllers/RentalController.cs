using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using RentalManagementSystem.Data;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Controllers
{
    public class RentalController
    {
        private readonly DatabaseConnection db = new DatabaseConnection();

        public List<Rental> GetAll()
        {
            var list = new List<Rental>();
            using var conn = db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT r.RentalID, r.PropertyID, r.TenantID, r.StartDate, r.EndDate, r.Status,
                       u.FullName AS TenantName, p.Address AS PropertyAddress
                FROM Rentals r
                INNER JOIN Tenants t ON r.TenantID = t.TenantID
                INNER JOIN Users u ON t.UserID = u.UserID
                INNER JOIN Properties p ON r.PropertyID = p.PropertyID
                ORDER BY r.StartDate DESC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Rental
                {
                    RentalID = (int)r["RentalID"],
                    PropertyID = (int)r["PropertyID"],
                    TenantID = (int)r["TenantID"],
                    StartDate = (DateTime)r["StartDate"],
                    EndDate = (DateTime)r["EndDate"],
                    Status = r["Status"] == DBNull.Value ? "" : r["Status"].ToString()!,
                    TenantName = r["TenantName"].ToString() ?? "",
                    PropertyAddress = r["PropertyAddress"].ToString() ?? ""
                });
            }
            return list;
        }

        public string Add(Rental rental)
        {
            string error = Validate(rental);
            if (error != "") return error;
            try
            {
                using var conn = db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand(@"
                    INSERT INTO Rentals (PropertyID, TenantID, StartDate, EndDate, Status)
                    VALUES (@p, @t, @s, @e, @st)", conn);
                Fill(cmd, rental);
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException ex) { return "Database error: " + ex.Message; }
        }

        public string Update(Rental rental)
        {
            string error = Validate(rental);
            if (error != "") return error;
            try
            {
                using var conn = db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand(@"
                    UPDATE Rentals
                    SET PropertyID=@p, TenantID=@t, StartDate=@s, EndDate=@e, Status=@st
                    WHERE RentalID=@id", conn);
                Fill(cmd, rental);
                cmd.Parameters.AddWithValue("@id", rental.RentalID);
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException ex) { return "Database error: " + ex.Message; }
        }

        public string Delete(int rentalId)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand("DELETE FROM Rentals WHERE RentalID=@id", conn);
                cmd.Parameters.AddWithValue("@id", rentalId);
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException)
            {
                return "Cannot delete: this rental has payments linked to it.";
            }
        }

        private static string Validate(Rental r)
        {
            if (r.TenantID <= 0) return "Please choose a tenant.";
            if (r.PropertyID <= 0) return "Please choose a property.";
            if (r.EndDate <= r.StartDate) return "End date must be after the start date.";
            return "";
        }

        private static void Fill(SqlCommand cmd, Rental r)
        {
            cmd.Parameters.AddWithValue("@p", r.PropertyID);
            cmd.Parameters.AddWithValue("@t", r.TenantID);
            cmd.Parameters.AddWithValue("@s", r.StartDate.Date);
            cmd.Parameters.AddWithValue("@e", r.EndDate.Date);
            cmd.Parameters.AddWithValue("@st", r.Status);
        }
    }
}