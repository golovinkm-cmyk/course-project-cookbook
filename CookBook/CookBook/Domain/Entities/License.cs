namespace Domain.Entities;

public class License
{
    public int Id { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty; // "Месячная", "Годовая"
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal Amount { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? ActivationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TransactionId { get; set; }
    public string? CardLastFour { get; set; }

    // Вычисляемое свойство для проверки валидности лицензии
    public bool IsValid()
    {
        if (!IsActive) return false;

        if (!ExpiryDate.HasValue) return true; // Бессрочная лицензия

        return ExpiryDate.Value > DateTime.Now;
    }
}