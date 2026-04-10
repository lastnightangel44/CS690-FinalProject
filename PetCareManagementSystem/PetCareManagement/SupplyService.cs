using PetCareManagementSystem.Models;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Services
{
    /// <summary>
    /// Manages supply records and calculates how much time remains before a supply runs out.
    /// Supplies are stored as pipe-delimited lines: Name|PurchaseDate|DurationDays
    /// </summary>
    public class SupplyService
    {
        private FileStorageService storage = new FileStorageService();

        // Supplies with this many days or fewer remaining are considered "low"
        private const int LowSupplyThresholdDays = 3;

        /// <summary>
        /// Saves a new supply record to the supplies file.
        /// </summary>
        public void AddSupply(Supply supply)
        {
            storage.Save(
                FilePaths.SuppliesFile,
                $"{supply.Name}|{supply.PurchaseDate}|{supply.DurationDays}"
            );
        }

        /// <summary>
        /// Loads and returns all supply records from file.
        /// </summary>
        public List<Supply> GetSupplies()
        {
            var lines = storage.Load(FilePaths.SuppliesFile);
            var supplies = new List<Supply>();

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                supplies.Add(new Supply
                {
                    Name            = parts[0],
                    PurchaseDate    = DateTime.Parse(parts[1]),
                    DurationDays    = int.Parse(parts[2])
                });
            }

            return supplies;
        }

        /// <summary>
        /// Calculates the number of whole days remaining before a supply is depleted.
        /// Returns a negative value if the supply has already expired.
        /// </summary>

        public int GetDaysRemaining(Supply supply)
        {
            DateTime endDate = supply.PurchaseDate.AddDays(supply.DurationDays);
            return (endDate - DateTime.Now).Days;
        }

        /// <summary>
        /// Returns all supplies that are running low (at or below the threshold).
        /// </summary>
        public List<Supply> GetLowSupplies()
        {
            var supplies = GetSupplies();
            var lowSupplies = new List<Supply>();

            foreach (var supply in supplies)
            {
                if (GetDaysRemaining(supply) <= LowSupplyThresholdDays)
                    lowSupplies.Add(supply);
            }

            return lowSupplies;
        }
    }
}