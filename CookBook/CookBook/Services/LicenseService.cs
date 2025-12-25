
using Data.Interfaces;


namespace Services;

public class LicenseService
{
    private readonly ILicenseRepository _licenseRepository;

    public LicenseService(ILicenseRepository licenseRepository)
    {
        _licenseRepository = licenseRepository;
    }

    public bool IsPremiumActive()
    {
        var activeLicenses = _licenseRepository.GetActiveLicenses();
        return activeLicenses.Any(l => l.IsValid());
    }

    public Domain.Entities.License? ActivateLicense(string licenseKey, string customerName, string customerEmail)
    {
        var license = _licenseRepository.GetByLicenseKey(licenseKey);
        if (license == null) return null;

        if (!license.IsValid()) return null;

        license.CustomerName = customerName;
        license.CustomerEmail = customerEmail;
        license.ActivationDate = DateTime.Now;

        if (license.LicenseType == "Годовая")
            license.ExpiryDate = DateTime.Now.AddYears(1);
        else if (license.LicenseType == "Месячная")
            license.ExpiryDate = DateTime.Now.AddMonths(1);

        _licenseRepository.Update(license);
        return license;
    }

    public Domain.Entities.License PurchaseLicense(string licenseType, string customerName,
        string customerEmail, decimal amount, string cardLastFour)
    {
        var license = new Domain.Entities.License
        {
            LicenseKey = GenerateLicenseKey(),
            LicenseType = licenseType,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            Amount = amount,
            CardLastFour = cardLastFour,
            IsActive = true
        };

        if (licenseType == "Годовая")
            license.ExpiryDate = DateTime.Now.AddYears(1);
        else if (licenseType == "Месячная")
            license.ExpiryDate = DateTime.Now.AddMonths(1);

        _licenseRepository.Add(license);
        return license;
    }

    private string GenerateLicenseKey()
    {
        return Guid.NewGuid().ToString().ToUpper().Replace("-", "").Substring(0, 20);
    }
}
