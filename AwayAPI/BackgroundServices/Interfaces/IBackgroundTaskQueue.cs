namespace AwayAPI.BackgroundServices.Interfaces;

public interface IBackgroundTaskQueue
{
    public ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);

    public ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}