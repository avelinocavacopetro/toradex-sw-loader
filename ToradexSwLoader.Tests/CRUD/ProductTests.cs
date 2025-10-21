using Blazored.Toast.Services;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using ToradexSwLoader.Components.Pages.Manage.CRUDDevices;
using ToradexSwLoader.Components.Pages.Manage.CRUDProducts;
using ToradexSwLoader.Data;
using ToradexSwLoader.Locales;
using ToradexSwLoader.Models;
using ToradexSwLoader.Services;

namespace ToradexSwLoader.Tests.CRUD
{
    public class ProductTests : TestContext
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.Products.Add(new Product { Id = 1, Name = "TestProduct1", Enabled = true });
            context.Products.Add(new Product { Id = 2, Name = "TestProduct2", Enabled = false });
            context.Products.Add(new Product { Id = 3, Name = "TestProduct3", Enabled = true });
            context.Products.Add(new Product { Id = 4, Name = "TestProduct4", Enabled = false });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void GetEnabledProducts_ReturnsOnlyEnabledProducts()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            // Act
            var enabledProducts = context.Products.Where(p => p.Enabled).ToList();

            // Assert
            Assert.Equal(2, enabledProducts.Count);
        }

        [Fact]
        public void AddProduct_AddsProductSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var newProduct = new Product { Id = 5, Name = "TestProuct5", Enabled = true };

            // Act
            context.Products.Add(newProduct);
            context.SaveChanges();

            // Assert
            var addedProduct = context.Products.Find(5);
            Assert.NotNull(addedProduct);
            Assert.Equal("TestProuct5", addedProduct.Name);
        }

        [Fact]
        public void UpdateProduct_UpdatesProuctSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var product = context.Products.Find(1);
            product!.Name = "UpdatedProductName";

            // Act
            context.Products.Update(product);
            context.SaveChanges();

            // Assert
            var updatedProduct = context.Products.Find(1);
            Assert.Equal("UpdatedProductName", updatedProduct?.Name);
        }

        [Fact]
        public void DeactivateProduct_DeactivatesProductSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var product = context.Products.Where(p => p.Enabled).FirstOrDefault();
            product!.Enabled = false;

            // Act
            context.Products.Update(product);
            context.SaveChanges();

            // Assert
            var deactivatedProduct = context.Products.Where(p => !p.Enabled).FirstOrDefault();
            Assert.False(deactivatedProduct!.Enabled);
        }
    }
}
