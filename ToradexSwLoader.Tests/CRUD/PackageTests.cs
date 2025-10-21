using Blazored.Toast.Services;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using ToradexSwLoader.Components.Pages.Manage.CRUDDevices;
using ToradexSwLoader.Data;
using ToradexSwLoader.Locales;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Tests.CRUD
{
    public class PackageTests : TestContext
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.Packages.Add(new Package { Id = "PackageId1", Name = "TestPackage1", Version = "Version1", Uri = "" });
            context.Packages.Add(new Package { Id = "PackageId2", Name = "TestPackage2", Version = "Version2", Uri = "" });
            context.Packages.Add(new Package { Id = "PackageId3", Name = "TestPackage3", Version = "Version3", Uri = "" });
            context.Packages.Add(new Package { Id = "PackageId4", Name = "TestPackage4", Version = "Version4", Uri = "" });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void GetPackages_ReturnsPackages()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            // Act
            var packages = context.Packages.ToList();

            // Assert
            Assert.Equal(4, packages.Count);
        }

        [Fact]
        public void AddPackage_AddsPackageSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var newPackage = new Package { Id = "PackageId5", Name = "TestPackage5", Version = "Version5", Uri = "" };

            // Act
            context.Packages.Add(newPackage);
            context.SaveChanges();

            // Assert
            var addedPackage = context.Packages.Find("PackageId5");
            Assert.NotNull(addedPackage);
            Assert.Equal("TestPackage5", addedPackage.Name);
        }

        [Fact]
        public void UpdatePackage_UpdatesPackageSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var package = context.Packages.Find("PackageId1");
            package!.Name = "UpdatedPackageName";

            // Act
            context.Packages.Update(package);
            context.SaveChanges();

            // Assert
            var updatedPackage = context.Packages.Find("PackageId1");
            Assert.Equal("UpdatedPackageName", updatedPackage?.Name);
        }

        [Fact]
        public void RemovePackage_RemovesPackageSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var package = context.Packages.Find("PackageId1");

            // Act
            context.Packages.Remove(package!);
            context.SaveChanges();

            // Assert
            var removedPackage = context.Packages.Find("PackageId1");
            Assert.Null(removedPackage);
        }
    }
}
