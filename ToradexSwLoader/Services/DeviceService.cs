using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Data;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class DeviceService
    {
        readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        readonly TorizonService _torizonService;

        public DeviceService(IDbContextFactory<AppDbContext> dbContextFactory, TorizonService torizonService)
        {
            _dbContextFactory = dbContextFactory;
            _torizonService = torizonService;
        }

        public async Task<bool> ImportDevicesFromApiAsync(string apiUrl)
        {
            bool authOk = await _torizonService.AuthenticateAsync();
            if (!authOk) return false;

            var devices = await _torizonService.GetItemsAsync<DetailedDevice>(apiUrl);
            if (devices == null) return false;

            using var context = _dbContextFactory.CreateDbContext();

            foreach (var device in devices)
            {
                var deviceDb = await context.DetailedDevices.FindAsync(device.DeviceUuid);
                if (deviceDb == null)
                {
                    context.DetailedDevices.Add(device);
                }
                else
                {
                    deviceDb.DeviceName = device.DeviceName;
                    deviceDb.DeviceId = device.DeviceId;
                    deviceDb.LastSeen = device.LastSeen;
                    deviceDb.CreatedAt = device.CreatedAt;
                    deviceDb.ActivatedAt = device.ActivatedAt;
                    deviceDb.DeviceStatus = device.DeviceStatus;
                    deviceDb.Notes = device.Notes;
                    deviceDb.Hibernated = device.Hibernated;

                    context.DetailedDevices.Update(deviceDb);
                }
            }

            await context.SaveChangesAsync();
            return true;
        }
    }
}