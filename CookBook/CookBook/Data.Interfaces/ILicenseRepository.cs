using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Data.Interfaces
{
    public interface ILicenseRepository
    {
        int Add(License license);
        License? GetById(int id);
        List<License> GetAll();
        bool Update(License license);
        bool Delete(int id);

        
        License? GetByLicenseKey(string licenseKey);
        bool IsLicenseActive(string licenseKey);
        bool ActivateLicense(string licenseKey);
    }
}
