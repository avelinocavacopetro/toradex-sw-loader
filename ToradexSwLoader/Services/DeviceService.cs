using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Data;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class DeviceService
    {
        private readonly AppDbContext _appDbContext;
        private readonly TorizonService _torizonService;

        public DeviceService(AppDbContext appDbContext, TorizonService torizonService)
        {
            _appDbContext = appDbContext;
            _torizonService = torizonService;
        }

        public async Task<bool> ImportDevicesFromApiAsync(string apiUrl)
        {
            bool authOk = await _torizonService.AuthenticateAsync();
            if (!authOk) return false;

            var devices = await _torizonService.GetItemsAsync<Device>(apiUrl);
            if (devices == null) return false;

            foreach (var device in devices)
            {
                var deviceDb = await _appDbContext.Devices.FindAsync(device.DeviceUuid);
                if (deviceDb == null)
                {
                    _appDbContext.Devices.Add(device);
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

                    _appDbContext.Devices.Update(deviceDb);
                }
            }

            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}