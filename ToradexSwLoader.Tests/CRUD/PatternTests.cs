using Blazored.Toast.Services;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using ToradexSwLoader.Components.Pages.Manage.CRUDDevices;
using ToradexSwLoader.Components.Pages.Manage.CRUDPatterns;
using ToradexSwLoader.Data;
using ToradexSwLoader.Locales;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Tests.CRUD
{
    public class PatternTests : TestContext
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.Patterns.Add(new Pattern { Id = 1, NameContains = "TestPattern1", Enabled = true });
            context.Patterns.Add(new Pattern { Id = 2, NameContains = "TestPattern2", Enabled = false });
            context.Patterns.Add(new Pattern { Id = 3, NameContains = "TestPattern3", Enabled = true });
            context.Patterns.Add(new Pattern { Id = 4, NameContains = "TestPattern4", Enabled = false });

            context.SaveChanges();

            return context;
        }

        private void PrepareMocks()
        {
            // Mock do ToastService
            var toastMock = new Mock<IToastService>();
            Services.AddSingleton(toastMock.Object);

            // Mock do Localizer
            var localizerMock = new Mock<IStringLocalizer<Resource>>();
            Services.AddSingleton(localizerMock.Object);

            // Mock do DbContextFactory
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = GetInMemoryDbContext();

            var dbFactoryMock = new Mock<IDbContextFactory<AppDbContext>>();
            dbFactoryMock.Setup(f => f.CreateDbContext()).Returns(context);
            Services.AddSingleton(dbFactoryMock.Object);
        }

        [Fact]
        public void GetEnabledPatterns_ReturnsOnlyEnabledPatterns()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            // Act
            var enabledPatterns = context.Patterns.Where(p => p.Enabled).ToList();

            // Assert
            Assert.Equal(2, enabledPatterns.Count);
        }

        [Fact]
        public void AddPattern_AddsPatternsSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var newPattern = new Pattern { Id = 5, NameContains = "TestPattern5", Enabled = true };

            // Act
            context.Patterns.Add(newPattern);
            context.SaveChanges();

            // Assert
            var addedPattern = context.Patterns.Find(5);
            Assert.NotNull(addedPattern);
            Assert.Equal("TestPattern5", addedPattern.NameContains);
        }

        [Fact]
        public void UpdatePattern_UpdatesPatternSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var pattern = context.Patterns.Find(1);
            pattern!.NameContains = "UpdatedPatternName";

            // Act
            context.Patterns.Update(pattern);
            context.SaveChanges();

            // Assert
            var updatedPattern = context.Patterns.Find(1);
            Assert.Equal("UpdatedPatternName", updatedPattern?.NameContains);
        }

        [Fact]
        public void DeactivatePattern_DeactivatesPatternsSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var pattern = context.Patterns.Where(p => p.Enabled).FirstOrDefault();
            pattern!.Enabled = false;

            // Act
            context.Patterns.Update(pattern);
            context.SaveChanges();

            // Assert
            var deactivatedPattern = context.Patterns.Where(p => !p.Enabled).FirstOrDefault();
            Assert.False(deactivatedPattern!.Enabled);
        }

        [Fact]
        public void ValidatePattern()
        {
            // Arrange

            PrepareMocks();

            var cut = RenderComponent<AddPattern>();

            // Act
            cut.Instance.pattern = "";
            var result = cut.Instance.ValidadePattern();

            // Assert
            Assert.False(result);
        }
    }
}
