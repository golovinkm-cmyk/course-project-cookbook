using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Data.Interfaces
{
    public interface ILicenseRepository : IRepository<License>
    {
        License? GetByLicenseKey(string licenseKey);
        IEnumerable<License> GetActiveLicenses();
        bool DeactivateLicense(int id);
        bool IsValidLicense(string licenseKey);
    }
}
