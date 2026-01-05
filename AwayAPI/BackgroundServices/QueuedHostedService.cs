namespace AwayAPI.BackgroundServices;
using AwayAPI.BackgroundServices.Interfaces;

public class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IServiceProvider _serviceProvider;

    public QueuedHostedService(IBackgroundTaskQueue taskQueue, IServiceProvider serviceProvider)
    {
        _taskQueue = taskQueue;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Wait for a job from the queue
            // Note that the loop only continues if an item is in the queue, otherwise execution will wait here.
            var workItem = await _taskQueue.DequeueAsync(stoppingToken);

            // Execute the job inside a fresh scope
            // Fresh scope is needed so that a fresh db conbtext is used in the processing logic. 
            try
            {
                using var scope = _serviceProvider.CreateScope();
                await workItem(stoppingToken); 
            }catch(Exception ex)
            {
                Console.WriteLine($"Failed to execute task in background service layer: {ex.Message}");
            }
        }
    }
}