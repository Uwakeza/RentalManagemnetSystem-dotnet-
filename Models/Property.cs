namespace RentalManagementSystem.Models
{
    public class Property
    {
        public int PropertyID { get; set; }
        public int OwnerID { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal RentAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;   // display only
    }
}