using System;

namespace Domain
{
    public class License
    {
        public int Id { get; set; }
        public string LicenseKey { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public DateTime ActivationDate { get; set; } = DateTime.Now;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Ожидание";
        public string? TransactionId { get; set; }
        public string CardLastFour { get; set; } = string.Empty;

        public License() { }

        public License(string licenseKey, string licenseType, decimal amount, string customerName, string customerEmail)
        {
            LicenseKey = licenseKey;
            LicenseType = licenseType;
            Amount = amount;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            IsActive = true;
            PaymentStatus = "Успешно";
        }
    }
}
