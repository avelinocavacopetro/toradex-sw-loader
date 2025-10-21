using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Data;

namespace ToradexSwLoader.Services
{
    public class DeviceStatusUpdaterService : BackgroundService
    {
        readonly IServiceProvider _serviceProvider;
        readonly TorizonService _torizonService;
        readonly ILogger<DeviceStatusUpdaterService> _logger;

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
            using (var scope = _serviceProvider.CreateScope())
            {
                var filterService = scope.ServiceProvider.GetRequiredService<FilterService>();
                await filterService.LoadFilterAsync();
            }

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

                int delaySeconds;

                using (var scope = _serviceProvider.CreateScope())
                {
                    var filterService = scope.ServiceProvider.GetRequiredService<FilterService>();
                    delaySeconds = filterService.RefreshTime;
                }

                if (delaySeconds < 10 || delaySeconds > 60)
                {
                    delaySeconds = 10;
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        async Task UpdateDeviceStatusesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var finalProducts = await dbContext.FinalProducts
                .Where(p => p.Enabled)
                .ToListAsync();

            if (!await _torizonService.AuthenticateAsync())
            {
                _logger.LogWarning("Falha na autenticação Torizon.");
                return;
            }

            bool anyUpdated = false;

            foreach (var fp in finalProducts)
            {
                var url = $"https://app.torizon.io/api/v2beta/devices/{fp.DeviceUuid}";
                string? statusAtual = await _torizonService.GetDeviceStatusAsync(url);

                if (!string.IsNullOrEmpty(statusAtual) && fp.Status != statusAtual)
                {
                    fp.Status = statusAtual;
                    _logger.LogInformation($"Status atualizado: {fp.DeviceUuid} => {statusAtual}");
                    dbContext.Update(fp);
                    anyUpdated = true;
                }
            }

            if (anyUpdated)
            {
                await dbContext.SaveChangesAsync();

                var stateService = scope.ServiceProvider.GetRequiredService<FinalProductStateService>();
                stateService.NotifyChanged();
            }
        }
    }
}