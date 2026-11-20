using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using RentalManagementSystem.Data;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Controllers
{
    public class PropertyController
    {
        private readonly DatabaseConnection db = new DatabaseConnection();

        public List<User> GetOwners()
        {
            var list = new List<User>();
            using var conn = db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT UserID, FullName FROM Users ORDER BY FullName", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new User { UserID = (int)r["UserID"], FullName = r["FullName"].ToString() ?? "" });
            return list;
        }

        public List<Property> GetAll()
        {
            var list = new List<Property>();
            using var conn = db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT p.PropertyID, p.OwnerID, p.Address, p.City, p.RentAmount, p.Description, u.FullName
                FROM Properties p
                INNER JOIN Users u ON p.OwnerID = u.UserID
                ORDER BY p.Address", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Property
                {
                    PropertyID = (int)r["PropertyID"],
                    OwnerID = (int)r["OwnerID"],
                    Address = r["Address"].ToString() ?? "",
                    City = r["City"] == DBNull.Value ? "" : r["City"].ToString()!,
                    RentAmount = (decimal)r["RentAmount"],
                    Description = r["Description"] == DBNull.Value ? "" : r["Description"].ToString()!,
                    OwnerName = r["FullName"].ToString() ?? ""
                });
            }
            return list;
        }

        public string Add(Property p)
        {
            string error = Validate(p);
            if (error != "") return error;
            try
            {
                using var conn = db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand(@"
                    INSERT INTO Properties (OwnerID, Address, City, RentAmount, Description)
                    VALUES (@o, @a, @c, @r, @d)", conn);
                Fill(cmd, p);
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException ex) { return "Database error: " + ex.Message; }
        }

        public string Update(Property p)
        {
            string error = Validate(p);
            if (error != "") return error;
            try
            {
                using var conn = db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand(@"
                    UPDATE Properties
                    SET OwnerID=@o, Address=@a, City=@c, RentAmount=@r, Description=@d
                    WHERE PropertyID=@id", conn);
                Fill(cmd, p);
                cmd.Parameters.AddWithValue("@id", p.PropertyID);
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException ex) { return "Database error: " + ex.Message; }
        }

        public string Delete(int propertyId)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();
                using var cmd = new SqlCommand("DELETE FROM Properties WHERE PropertyID=@id", conn);
                cmd.Parameters.AddWithValue("@id", propertyId);
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException)
            {
                return "Cannot delete: this property has rentals linked to it.";
            }
        }

        private static string Validate(Property p)
        {
            if (string.IsNullOrWhiteSpace(p.Address)) return "Address is required.";
            if (p.RentAmount <= 0) return "Rent amount must be greater than 0.";
            if (p.OwnerID <= 0) return "Please choose an owner.";
            return "";
        }

        private static void Fill(SqlCommand cmd, Property p)
        {
            cmd.Parameters.AddWithValue("@o", p.OwnerID);
            cmd.Parameters.AddWithValue("@a", p.Address.Trim());
            cmd.Parameters.AddWithValue("@c", p.City.Trim());
            cmd.Parameters.AddWithValue("@r", p.RentAmount);
            cmd.Parameters.AddWithValue("@d", p.Description.Trim());
        }
    }
}