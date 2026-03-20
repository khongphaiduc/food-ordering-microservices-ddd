
namespace tracking_service.Tracking.Infastructrure.Worker
{
    public class TrakingComsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScope;

        public TrakingComsumer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScope = serviceScopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}
