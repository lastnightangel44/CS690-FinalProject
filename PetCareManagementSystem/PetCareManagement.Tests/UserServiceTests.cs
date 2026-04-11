using System.Collections.Generic;
using Xunit;
using PetCareManagementSystem.Models;
using PetCareManagementSystem.Services;

namespace PetCareManagementSystem.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public void CreateUser_SavesTheUser()
        {
            TestHelper.ResetTestFiles();
            var service = new UserService();

            service.CreateUser("Alice");
            List<User> users = service.GetUsers();

            Assert.Single(users);
            Assert.Equal("Alice", users[0].Name);
        }

        [Fact]
        public void UpdateUser_ChangesTheName()
        {
            TestHelper.ResetTestFiles();
            var service = new UserService();

            service.CreateUser("Bob");
            User user = service.GetUsers()[0];

            user.Name = "Robert";
            service.UpdateUser(user);

            Assert.Equal("Robert", service.GetUsers()[0].Name);
        }

        [Fact]
        public void DeleteUser_RemovesTheUser()
        {
            TestHelper.ResetTestFiles();
            var service = new UserService();

            service.CreateUser("Charlie");
            User user = service.GetUsers()[0];

            service.DeleteUser(user.Id);

            Assert.Empty(service.GetUsers());
        }
    }
}
