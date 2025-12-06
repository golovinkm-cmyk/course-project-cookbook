using Domain;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Data.InMemory
{
    public class LicenseRepository : ILicenseRepository
    {
        private readonly List<License> _licenses = new();
        private int _nextId = 1;

        public LicenseRepository()
        {
            // Тестовые лицензии
            SeedData();
        }

        private void SeedData()
        {
            Add(new License
            {
                LicenseKey = "ABCD-1234-EFGH-5678",
                LicenseType = "Пожизненная",
                PurchaseDate = new DateTime(2024, 1, 15),
                ActivationDate = new DateTime(2024, 1, 15),
                ExpiryDate = null,
                IsActive = true,
                CustomerName = "Иван Иванов",
                CustomerEmail = "ivan@example.com",
                Amount = 999,
                PaymentMethod = "Карта",
                PaymentStatus = "Успешно",
                TransactionId = "TXN123456",
                CardLastFour = "1234"
            });
        }

        public int Add(License license)
        {
            license.Id = _nextId++;
            license.PurchaseDate = DateTime.Now;
            if (license.ActivationDate == DateTime.MinValue)
                license.ActivationDate = DateTime.Now;

            _licenses.Add(license);
            return license.Id;
        }

        public License? GetById(int id)
        {
            return _licenses.FirstOrDefault(l => l.Id == id);
        }

        public List<License> GetAll()
        {
            return _licenses.ToList();
        }

        public bool Update(License license)
        {
            var existing = GetById(license.Id);
            if (existing == null) return false;

            existing.LicenseKey = license.LicenseKey;
            existing.LicenseType = license.LicenseType;
            existing.ActivationDate = license.ActivationDate;
            existing.ExpiryDate = license.ExpiryDate;
            existing.IsActive = license.IsActive;
            existing.CustomerName = license.CustomerName;
            existing.CustomerEmail = license.CustomerEmail;
            existing.Amount = license.Amount;
            existing.PaymentMethod = license.PaymentMethod;
            existing.PaymentStatus = license.PaymentStatus;
            existing.TransactionId = license.TransactionId;
            existing.CardLastFour = license.CardLastFour;

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

        public bool IsLicenseActive(string licenseKey)
        {
            var license = GetByLicenseKey(licenseKey);
            if (license == null) return false;

            return license.IsActive &&
                   (license.ExpiryDate == null || license.ExpiryDate > DateTime.Now);
        }

        public bool ActivateLicense(string licenseKey)
        {
            var license = GetByLicenseKey(licenseKey);
            if (license == null) return false;

            license.IsActive = true;
            license.ActivationDate = DateTime.Now;
            return true;
        }
    }
}
