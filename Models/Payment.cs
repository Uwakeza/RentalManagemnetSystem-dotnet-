using System;

namespace RentalManagementSystem.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int RentalID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string RentalInfo { get; set; } = string.Empty;   // display only
    }

    // Small helper class for the rental dropdown
    public class RentalChoice
    {
        public int RentalID { get; set; }
        public string Display { get; set; } = string.Empty;
    }
}