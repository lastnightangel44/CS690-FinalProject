using System;
using System.Collections.Generic;
using Xunit;
using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.Tests
{
    public class SupplyServiceTests
    {
        [Fact]
        public void AddSupply_SavesTheSupply()
        {
            TestHelper.ResetTestFiles();
            var service = new SupplyService();

            service.AddSupply(new Supply
            {
                Name         = "Dog Food",
                PurchaseDate = DateTime.Now,
                DurationDays = 30
            });

            List<Supply> supplies = service.GetSupplies();

            Assert.Single(supplies);
            Assert.Equal("Dog Food", supplies[0].Name);
        }

        [Fact]
        public void GetLowSupplies_ReturnsSupplyWithOneDayLeft()
        {
            TestHelper.ResetTestFiles();
            var service = new SupplyService();

            // Purchased 29 days ago with a 30-day duration — 1 day left
            service.AddSupply(new Supply
            {
                Name         = "Flea Treatment",
                PurchaseDate = DateTime.Now.AddDays(-29),
                DurationDays = 30
            });

            List<Supply> low = service.GetLowSupplies();

            Assert.Single(low);
            Assert.Equal("Flea Treatment", low[0].Name);
        }

        [Fact]
        public void DeleteSupply_RemovesTheSupply()
        {
            TestHelper.ResetTestFiles();
            var service = new SupplyService();

            Supply supply = new Supply
            {
                Name         = "Cat Food",
                PurchaseDate = DateTime.Now,
                DurationDays = 14
            };

            service.AddSupply(supply);
            service.DeleteSupply(supply);

            Assert.Empty(service.GetSupplies());
        }
    }
}
