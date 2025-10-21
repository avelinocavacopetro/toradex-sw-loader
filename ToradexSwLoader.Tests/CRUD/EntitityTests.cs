using Blazored.Toast.Services;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Moq;
using ToradexSwLoader.Components.Pages.Manage.CRUDDevices;
using ToradexSwLoader.Components.Pages.Manage.CRUDEntities;
using ToradexSwLoader.Data;
using ToradexSwLoader.Locales;
using ToradexSwLoader.Models;
using ToradexSwLoader.Services;

namespace ToradexSwLoader.Tests.CRUD
{
    public class EntitityTests : TestContext
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.Entities.Add(new Entity { Id = 1, Name = "TestEntity1", Observation = "", Enabled = true });
            context.Entities.Add(new Entity { Id = 2, Name = "TestEntity2", Observation = "", Enabled = false });
            context.Entities.Add(new Entity { Id = 3, Name = "TestEntity3", Observation = "", Enabled = true });
            context.Entities.Add(new Entity { Id = 4, Name = "TestEntity4", Observation = "", Enabled = false });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void GetEnabledEntities_ReturnsOnlyEnabledDevices()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            // Act
            var enabledEntities = context.Entities.Where(d => d.Enabled).ToList();

            // Assert
            Assert.Equal(2, enabledEntities.Count);
        }

        [Fact]
        public void AddEntity_AddsEntitySuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var newEntity = new Entity { Id = 5, Name = "TestEntity5", Observation = "", Enabled = true };

            // Act
            context.Entities.Add(newEntity);
            context.SaveChanges();

            // Assert
            var addedEntity = context.Entities.Find(5);
            Assert.NotNull(addedEntity);
            Assert.Equal("TestEntity5", addedEntity.Name);
        }

        [Fact]
        public void UpdateEntity_UpdatesEntitySuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var entity = context.Entities.Find(1);
            entity!.Name = "UpdatedEntityName";

            // Act
            context.Entities.Update(entity);
            context.SaveChanges();

            // Assert
            var updatedEntity = context.Entities.Find(1);
            Assert.Equal("UpdatedEntityName", updatedEntity?.Name);
        }

        [Fact]
        public void DeactivateEntity_DeactivatesEntitySuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var entity = context.Entities.Where(e => e.Enabled).FirstOrDefault();
            entity!.Enabled = false;

            // Act
            context.Entities.Update(entity);
            context.SaveChanges();

            // Assert
            var deactivatedEntity = context.Entities.Where(e => !e.Enabled).FirstOrDefault();
            Assert.False(deactivatedEntity!.Enabled);
        }
    }
}
