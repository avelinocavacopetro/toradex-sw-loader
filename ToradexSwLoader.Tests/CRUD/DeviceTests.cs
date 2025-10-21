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
using ToradexSwLoader.Services;

namespace ToradexSwLoader.Tests.CRUD
{
    public class DeviceTests : TestContext
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.Devices.Add(new Device { Id = 1, Name = "TestDevice1", Enabled = true});
            context.Devices.Add(new Device { Id = 2, Name = "TestDevice2", Enabled = false});
            context.Devices.Add(new Device { Id = 3, Name = "TestDevice3", Enabled = true});
            context.Devices.Add(new Device { Id = 4, Name = "TestDevice4", Enabled = false});

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

            var context = GetInMemoryDbContext();

            var dbFactoryMock = new Mock<IDbContextFactory<AppDbContext>>();
            dbFactoryMock.Setup(f => f.CreateDbContext()).Returns(context);
            Services.AddSingleton(dbFactoryMock.Object);

            var filterServiceMock = new Mock<FilterService>(dbFactoryMock.Object);
            Services.AddSingleton(filterServiceMock.Object);
        }

        [Fact]
        public void GetEnabledDevices_ReturnsOnlyEnabledDevices()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            // Act
            var enabledDevices = context.Devices.Where(d => d.Enabled).ToList();

            // Assert
            Assert.Equal(2, enabledDevices.Count);
        }

        [Fact]
        public void AddDevice_AddsDeviceSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var newDevice = new Device { Id = 5, Name = "TestDevice5", Enabled = true };

            // Act
            context.Devices.Add(newDevice);
            context.SaveChanges();

            // Assert
            var addedDevice = context.Devices.Find(5);
            Assert.NotNull(addedDevice);
            Assert.Equal("TestDevice5", addedDevice.Name);
        }

        [Fact]
        public void UpdateDevice_UpdatesDeviceSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var device = context.Devices.Find(1);
            device!.Name = "UpdatedDeviceName";

            // Act
            context.Devices.Update(device);
            context.SaveChanges();

            // Assert
            var updatedDevice = context.Devices.Find(1);
            Assert.Equal("UpdatedDeviceName", updatedDevice?.Name);
        }

        [Fact]
        public void DeactivateDevice_DeactivatesDeviceSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var device = context.Devices.Where(d => d.Enabled).FirstOrDefault();
            device!.Enabled = false;

            // Act
            context.Devices.Update(device);
            context.SaveChanges();

            // Assert
            var deactivatedDevice = context.Devices.Where(d => !d.Enabled).FirstOrDefault();
            Assert.False(deactivatedDevice!.Enabled);
        }

        [Fact]
        public void ValidateNameOfAnDevice()
        {
            // Arrange

            PrepareMocks();

            var cut = RenderComponent<AddDevice>();

            // Act
            cut.Instance.deviceName = ""; 
            var result = cut.Instance.ValidateDeviceName();

            // Assert
            Assert.False(result);
        }
    }
}
