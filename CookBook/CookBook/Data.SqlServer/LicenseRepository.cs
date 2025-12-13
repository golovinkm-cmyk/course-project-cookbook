using Data.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;


namespace Data.SqlServer
{
    public class LicenseRepository : ILicenseRepository
    {
        private readonly CookbookDbContext _context;

        public LicenseRepository(CookbookDbContext context)
        {
            _context = context;
        }

        public int Add(License license)
        {
            _context.Licenses.Add(license);
            _context.SaveChanges();
            return license.Id;
        }

        public License? GetById(int id)
        {
            return _context.Licenses.Find(id);
        }

        public List<License> GetAll()
        {
            return _context.Licenses.ToList();
        }

        public bool Update(License license)
        {
            var existing = _context.Licenses.Find(license.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(license);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var license = _context.Licenses.Find(id);
            if (license == null) return false;

            _context.Licenses.Remove(license);
            _context.SaveChanges();
            return true;
        }

        public License? GetByLicenseKey(string licenseKey)
        {
            return _context.Licenses
                .FirstOrDefault(l => l.LicenseKey.Equals(licenseKey, System.StringComparison.OrdinalIgnoreCase));
        }

        public bool IsLicenseActive(string licenseKey)
        {
            var license = GetByLicenseKey(licenseKey);
            if (license == null) return false;

            return license.IsActive &&
                   (license.ExpiryDate == null || license.ExpiryDate > System.DateTime.Now);
        }

        public bool ActivateLicense(string licenseKey)
        {
            var license = GetByLicenseKey(licenseKey);
            if (license == null) return false;

            license.IsActive = true;
            license.ActivationDate = System.DateTime.Now;
            _context.SaveChanges();
            return true;
        }
    }
}
