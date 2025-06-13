using SponsorshipApp.Application.Interfaces;
using SponsorshipApp.Application.Services;

namespace SponsorshipApp.API.Background
{
    public class SponsorshipScheduler : BackgroundService
    {
        private readonly IServiceScopeFactory _factory;

        public SponsorshipScheduler(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _factory.CreateScope();
                var planService = scope.ServiceProvider.GetRequiredService<ISponsorshipPlanService>();
                planService.ProcessRecurringPayments();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
