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
    public class StackTests : TestContext
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.Stacks.Add(new Stack { Id = 1, Name = "TestStack1", Enabled = true });
            context.Stacks.Add(new Stack { Id = 2, Name = "TestStack2", Enabled = false });
            context.Stacks.Add(new Stack { Id = 3, Name = "TestStack3", Enabled = true });
            context.Stacks.Add(new Stack { Id = 4, Name = "TestStack4", Enabled = false });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public void GetEnabledStacks_ReturnsOnlyEnabledStacks()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            // Act
            var enabledStacks = context.Stacks.Where(s => s.Enabled).ToList();

            // Assert
            Assert.Equal(2, enabledStacks.Count);
        }

        [Fact]
        public void AddStack_AddsStackSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var newStack = new Stack { Id = 5, Name = "TestStack5", Enabled = true };

            // Act
            context.Stacks.Add(newStack);
            context.SaveChanges();

            // Assert
            var addedStack = context.Stacks.Find(5);
            Assert.NotNull(addedStack);
            Assert.Equal("TestStack5", addedStack.Name);
        }

        [Fact]
        public void UpdateStack_UpdatesStackSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var stack = context.Stacks.Find(1);
            stack!.Name = "UpdatedStackName";

            // Act
            context.Stacks.Update(stack);
            context.SaveChanges();

            // Assert
            var updatedStack = context.Stacks.Find(1);
            Assert.Equal("UpdatedStackName", updatedStack?.Name);
        }

        [Fact]
        public void DeactivateStack_DeactivatesStackSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var stack = context.Stacks.Where(s => s.Enabled).FirstOrDefault();
            stack!.Enabled = false;

            // Act
            context.Stacks.Update(stack);
            context.SaveChanges();

            // Assert
            var deactivatedStacks = context.Stacks.Where(s => !s.Enabled).FirstOrDefault();
            Assert.False(deactivatedStacks!.Enabled);
        }
    }
}
