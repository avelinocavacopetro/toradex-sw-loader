using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Data;
using ToradexSwLoader.Models;


namespace ToradexSwLoader.Services
{
    public class DeviceStatusUpdaterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TorizonService _torizonService;
        private readonly ILogger<DeviceStatusUpdaterService> _logger;

        public DeviceStatusUpdaterService(
            IServiceProvider serviceProvider,
            TorizonService torizonService,
            ILogger<DeviceStatusUpdaterService> logger)
        {
            _serviceProvider = serviceProvider;
            _torizonService = torizonService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateDeviceStatusesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar estados dos dispositivos.");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task UpdateDeviceStatusesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var finalProducts = await dbContext.FinalProducts
                .Where(fp => fp.Status == "UpdatePending")
                .ToListAsync();

            if (!await _torizonService.AuthenticateAsync())
            {
                _logger.LogWarning("Falha na autenticação Torizon.");
                return;
            }

            foreach (var fp in finalProducts)
            {
                var url = $"https://app.torizon.io/api/v2beta/devices/{fp.DeviceUuid}";
                string? statusAtual = await _torizonService.GetDeviceStatusAsync(url);

                if (!string.IsNullOrEmpty(statusAtual) && fp.Status != statusAtual)
                {
                    fp.Status = statusAtual;
                    _logger.LogInformation($"Status atualizado: {fp.DeviceUuid} => {statusAtual}");
                    dbContext.Update(fp);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}