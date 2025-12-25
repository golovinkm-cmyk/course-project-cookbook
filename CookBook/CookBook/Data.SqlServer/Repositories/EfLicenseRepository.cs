using Data.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.SqlServer.Repositories;

public class EfLicenseRepository : ILicenseRepository
{
    private readonly CookBookDbContext _context;

    public EfLicenseRepository(CookBookDbContext context)
    {
        _context = context;
    }

    public License? GetById(int id)
    {
        return _context.Licenses.Find(id);
    }

    public IEnumerable<License> GetAll()
    {
        return _context.Licenses.AsNoTracking().ToList();
    }

    public int Add(License license)
    {
        license.PurchaseDate = DateTime.Now;
        _context.Licenses.Add(license);
        _context.SaveChanges();
        return license.Id;
    }

    public bool Update(License license)
    {
        var existing = GetById(license.Id);
        if (existing == null) return false;

        existing.LicenseKey = license.LicenseKey;
        existing.LicenseType = license.LicenseType;
        existing.CustomerName = license.CustomerName;
        existing.CustomerEmail = license.CustomerEmail;
        existing.IsActive = license.IsActive;
        existing.Amount = license.Amount;
        existing.ExpiryDate = license.ExpiryDate;
        existing.ActivationDate = license.ActivationDate;
        existing.CardLastFour = license.CardLastFour;

        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var license = GetById(id);
        if (license == null) return false;

        _context.Licenses.Remove(license);
        _context.SaveChanges();
        return true;
    }

    public License? GetByLicenseKey(string licenseKey)
    {
        return _context.Licenses
            .AsNoTracking()
            .FirstOrDefault(l => l.LicenseKey == licenseKey);
    }

    public IEnumerable<License> GetActiveLicenses()
    {
        var now = DateTime.Now;
        return _context.Licenses
            .Where(l => l.IsActive &&
                       (!l.ExpiryDate.HasValue || l.ExpiryDate.Value > now))
            .AsNoTracking()
            .ToList();
    }

    public bool DeactivateLicense(int id)
    {
        var license = GetById(id);
        if (license == null) return false;

        license.IsActive = false;
        _context.SaveChanges();
        return true;
    }

    public bool IsValidLicense(string licenseKey)
    {
        var license = GetByLicenseKey(licenseKey);
        if (license == null) return false;

        return license.IsActive && license.IsValid();
    }
}