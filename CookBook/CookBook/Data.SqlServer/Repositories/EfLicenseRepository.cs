using Data.Interfaces;
using Domain.Entities;
using Data.SqlServer;


namespace CookBook.Data.SqlServer.Repositories;

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
        return _context.Licenses.ToList();
    }

    public int Add(License license)
    {
        license.PurchaseDate = DateTime.Now;
        license.ActivationDate = DateTime.Now;
        _context.Licenses.Add(license);
        _context.SaveChanges();
        return license.Id;
    }

    public bool Update(License license)
    {
        var existing = GetById(license.Id);
        if (existing == null) return false;

        _context.Entry(existing).CurrentValues.SetValues(license);
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
            .FirstOrDefault(l => l.LicenseKey == licenseKey);
    }

    public IEnumerable<License> GetActiveLicenses()
    {
        return _context.Licenses
            .Where(l => l.IsActive)
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
        return license != null && license.IsValid();
    }
}
