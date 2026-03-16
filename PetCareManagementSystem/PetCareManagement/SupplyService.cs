using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    public class SupplyService
    {
        private FileStorageService storage = new FileStorageService();

        public void AddSupply(Supply supply)
        {
            storage.Save(
                FilePaths.SuppliesFile,
                $"{supply.Name}|{supply.PurchaseDate}|{supply.DurationDays}"
            );
        }

        public List<Supply> GetSupplies()
        {
            var lines = storage.Load(FilePaths.SuppliesFile);
            var supplies = new List<Supply>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                supplies.Add(new Supply
                {
                    Name = parts[0],
                    PurchaseDate = DateTime.Parse(parts[1]),
                    DurationDays = int.Parse(parts[2])
                });
            }

            return supplies;
        }

        public int GetDaysRemaining(Supply supply)
        {
            DateTime endDate = supply.PurchaseDate.AddDays(supply.DurationDays);

            return (endDate - DateTime.Now).Days;
        }

        public List<Supply> GetLowSupplies()
        {
            var supplies = GetSupplies();
            var lowSupplies = new List<Supply>();

            foreach (var supply in supplies)
            {
                int daysRemaining = GetDaysRemaining(supply);

                if (daysRemaining <= 3)
                {
                    lowSupplies.Add(supply);
                }
            }

            return lowSupplies;
        }
    }
}