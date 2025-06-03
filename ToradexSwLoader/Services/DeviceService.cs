using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Data;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class DeviceService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly TorizonService _torizonService;

        public DeviceService(IDbContextFactory<AppDbContext> dbContextFactory, TorizonService torizonService)
        {
            _dbContextFactory = dbContextFactory;
            _torizonService = torizonService;
        }

        public async Task<bool> ImportDevicesFromApiAsync(string apiUrl)
        {
            bool authOk = await _torizonService.AuthenticateAsync();
            if (!authOk) return false;

            var devices = await _torizonService.GetItemsAsync<Device>(apiUrl);
            if (devices == null) return false;

            using var context = _dbContextFactory.CreateDbContext();

            foreach (var device in devices)
            {
                var deviceDb = await context.Devices.FindAsync(device.DeviceUuid);
                if (deviceDb == null)
                {
                    context.Devices.Add(device);
                }
                else
                {
                    // Atualiza campos
                    deviceDb.DeviceName = device.DeviceName;
                    deviceDb.DeviceId = device.DeviceId;
                    deviceDb.LastSeen = device.LastSeen;
                    deviceDb.CreatedAt = device.CreatedAt;
                    deviceDb.ActivatedAt = device.ActivatedAt;
                    deviceDb.DeviceStatus = device.DeviceStatus;
                    deviceDb.Notes = device.Notes;
                    deviceDb.Hibernated = device.Hibernated;

                    context.Devices.Update(deviceDb);
                }
            }

            await context.SaveChangesAsync();
            return true;
        }
    }
}