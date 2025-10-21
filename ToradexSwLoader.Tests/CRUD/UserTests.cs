using Blazored.Toast.Services;
using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using ToradexSwLoader.Components.Pages.Manage.CRUDUsers;
using ToradexSwLoader.Data;
using ToradexSwLoader.Locales;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Tests.CRUD
{
    public class UserTests : TestContext
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.Users.Add(new UserPetrotec { Id = 1, UserRoleId = 1, Entity = null, Enabled = true, UserName = "TestUser1", Email = "test1.email@petrotec.com", Password = "TestPassword1", Culture = "pt-PT" });
            context.Users.Add(new UserPetrotec { Id = 2, UserRoleId = 1, Entity = null, Enabled = false, UserName = "TestUser2", Email = "test2.email@petrotec.com", Password = "TestPassword2", Culture = "pt-PT" });
            context.Users.Add(new UserPetrotec { Id = 3, UserRoleId = 1, Entity = null, Enabled = true, UserName = "TestUser3", Email = "test3.email@petrotec.com", Password = "TestPassword3", Culture = "pt-PT" });
            context.Users.Add(new UserPetrotec { Id = 4, UserRoleId = 1, Entity = null, Enabled = false, UserName = "TestUser4", Email = "test4.email@petrotec.com", Password = "TestPassword4", Culture = "pt-PT" });

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
        public void GetEnabledUsers_ReturnsOnlyEnabledUsers()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            // Act
            var enabledUsers = context.Users.Where(u => u.Enabled).ToList();

            // Assert
            Assert.Equal(2, enabledUsers.Count);
        }

        [Fact]
        public void AddUser_AddsUserSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var newUser = new UserPetrotec { Id = 5, UserRoleId = 1, Entity = null, Enabled = false, UserName = "TestUser5", Email = "test5.email@petrotec.com", Password = "TestPassword5", Culture = "pt-PT" };

            // Act
            context.Users.Add(newUser);
            context.SaveChanges();

            // Assert
            var addedUser = context.Users.Find(5);
            Assert.NotNull(addedUser);
            Assert.Equal("TestUser5", addedUser.UserName);
        }

        [Fact]
        public void UpdateUser_UpdatesUserSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var user = context.Users.Find(1);
            user!.UserName = "UpdatedUserName";
            user!.Email = "updated.email@petrotec.com";
            user!.Password = "UpdatedPassword";

            // Act
            context.Users.Update(user);
            context.SaveChanges();

            // Assert
            var updatedUser = context.Users.Find(1);
            Assert.Equal("UpdatedUserName", updatedUser?.UserName);
            Assert.Equal("updated.email@petrotec.com", updatedUser?.Email);
            Assert.Equal("UpdatedPassword", updatedUser?.Password);
        }

        [Fact]
        public void DeactivateUser_DeactivatesUserSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var user = context.Users.Where(u => u.Enabled).FirstOrDefault();
            user!.Enabled = false;

            // Act
            context.Users.Update(user);
            context.SaveChanges();

            // Assert
            var deactivatedUser = context.Users.Where(u => !u.Enabled).FirstOrDefault();
            Assert.False(deactivatedUser!.Enabled);
        }

        [Fact]
        public void ValidateUser()
        {
            // Arrange

            PrepareMocks();

            var cut = RenderComponent<AddUser>();

            // Act
            cut.Instance.userName = "";
            cut.Instance.userEmail = "";
            cut.Instance.userPassword = "";
            var result = cut.Instance.ValidateUser();

            // Assert
            Assert.False(result);
        }
    }
}
