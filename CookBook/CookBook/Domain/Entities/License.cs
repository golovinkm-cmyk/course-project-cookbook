using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class License
    {
        public object PaymentStatus;
        public object TransactionId;

        public int Id { get; set; }
        public string LicenseKey { get; set; } = string.Empty;
        public string LicenseType { get; set; } = "Месячная";
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public DateTime ActivationDate { get; set; } = DateTime.Now;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CardLastFour { get; set; } = string.Empty;
        public object PaymentMethod { get; set; }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
