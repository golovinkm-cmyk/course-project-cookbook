using System.Xml.Linq;
using Data.Interfaces;
using Domain.Entities;


namespace Data.InMemory;

public class InMemoryLicenseRepository : ILicenseRepository
{
    private readonly List<License> _licenses = new();
    private int _nextId = 1;

    public License? GetById(int id)
    {
        return _licenses.FirstOrDefault(l => l.Id == id);
    }

    public IEnumerable<License> GetAll()
    {
        return _licenses;
    }

    public int Add(License license)
    {
        license.Id = _nextId++;
        license.PurchaseDate = DateTime.Now;
        license.ActivationDate = DateTime.Now;
        _licenses.Add(license);
        return license.Id;
    }

    public bool Update(License license)
    {
        var existing = GetById(license.Id);
        if (existing == null) return false;

        var index = _licenses.IndexOf(existing);
        _licenses[index] = license;
        return true;
    }

    public bool Delete(int id)
    {
        var license = GetById(id);
        if (license == null) return false;

        return _licenses.Remove(license);
    }

    public License? GetByLicenseKey(string licenseKey)
    {
        return _licenses.FirstOrDefault(l =>
            l.LicenseKey.Equals(licenseKey, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<License> GetActiveLicenses()
    {
        return _licenses.Where(l => l.IsActive && l.IsValid());
    }

    public bool DeactivateLicense(int id)
    {
        var license = GetById(id);
        if (license == null) return false;

        license.IsActive = false;
        return true;
    }

    public bool IsValidLicense(string licenseKey)
    {
        var license = GetByLicenseKey(licenseKey);
        return license != null && license.IsValid();
    }
}
