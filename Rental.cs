using System;

namespace RentalManagementSystem.Models
{
    public class Rental
    {
        public int RentalID { get; set; }
        public int PropertyID { get; set; }
        public int TenantID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;     // display only
        public string PropertyAddress { get; set; } = string.Empty; // display only
    }
}