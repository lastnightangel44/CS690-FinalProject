using System.Collections.Generic;
using System.IO;
using Xunit;
using PetCareManagementSystem.Data;

namespace PetCareManagementSystem.Tests
{
    public class FileStorageServiceTests
    {
        [Fact]
        public void Test_Save_Load()
        {
            TestHelper.ResetTestFiles();
            var storage = new FileStorageService();

            storage.Save(FilePaths.UsersFile, "hello|world");
            List<string> lines = storage.Load(FilePaths.UsersFile);

            Assert.Single(lines);
            Assert.Equal("hello|world", lines[0]);
        }

        [Fact]
        public void Test_DeleteLine()
        {
            TestHelper.ResetTestFiles();
            var storage = new FileStorageService();

            storage.Save(FilePaths.UsersFile, "keep|this");
            storage.Save(FilePaths.UsersFile, "remove|this");

            storage.DeleteLine(FilePaths.UsersFile, line => line.StartsWith("remove"));

            List<string> lines = storage.Load(FilePaths.UsersFile);

            Assert.Single(lines);
            Assert.Equal("keep|this", lines[0]);
        }

        [Fact]
        public void Test_UpdateLine()
        {
            TestHelper.ResetTestFiles();
            var storage = new FileStorageService();

            storage.Save(FilePaths.UsersFile, "aaa|old");

            storage.UpdateLine(FilePaths.UsersFile, line => line.StartsWith("aaa"), "aaa|new");

            List<string> lines = storage.Load(FilePaths.UsersFile);

            Assert.Equal("aaa|new", lines[0]);
        }
    }
}
