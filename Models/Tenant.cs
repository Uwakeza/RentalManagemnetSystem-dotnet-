namespace RentalManagementSystem.Models
{
    public class Tenant
    {
        public int TenantID { get; set; }
        public int UserID { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}