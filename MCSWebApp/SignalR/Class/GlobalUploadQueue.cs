using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public static class GlobalUploadQueue
{
    private static int _queuedTasks = 0;
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private static ConcurrentDictionary<string, int> _operationPositions = new ConcurrentDictionary<string, int>();
    private static IHubContext<ProgressHub> _hubContext;

    public static void Initialize(IHubContext<ProgressHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public static async Task<IDisposable> EnterQueueAsync(string operationId, IHubContext<ProgressHub> hubContext)
    {
        if (_hubContext == null)
        {
            _hubContext = hubContext;
        }

        int position = Interlocked.Increment(ref _queuedTasks) - 1;
        _operationPositions[operationId] = position;
        await UpdateAllClients();

        await _semaphore.WaitAsync();
        _operationPositions[operationId] = -1; // -1 indicates processing
        await UpdateAllClients();

        return new QueueExitHandler(async () =>
        {
            _semaphore.Release();
            _operationPositions.TryRemove(operationId, out _);
            Interlocked.Decrement(ref _queuedTasks);
            await UpdateAllClients();
        });
    }

    private static async Task UpdateAllClients()
    {
        foreach (var kvp in _operationPositions)
        {
            int actualPosition = kvp.Value == -1 ? -1 : _operationPositions.Count(x => x.Value != -1 && x.Value < kvp.Value);
            await _hubContext.Clients.Group(kvp.Key).SendAsync("QueueUpdate", actualPosition);
        }
    }

    private class QueueExitHandler : IDisposable
    {
        private Func<Task> _exitAction;

        public QueueExitHandler(Func<Task> exitAction)
        {
            _exitAction = exitAction;
        }

        public void Dispose()
        {
            _exitAction?.Invoke().Wait();
        }
    }
}