namespace AwayAPI.BackgroundServices.Interfaces;

public interface IBackgroundTaskQueue
{
    public ValueTask QueueBackgroundWorkItemAsync(Func<IServiceProvider, CancellationToken, ValueTask> workItem);

    public ValueTask<Func<IServiceProvider, CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}