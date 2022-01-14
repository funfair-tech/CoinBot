using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;

namespace CoinBot.Core;

public abstract class TickingService
{
    private readonly BufferBlock<Func<Task>> _bufferBlock;

    /// <summary>
    ///     The tick interval <see cref="TimeSpan" />.
    /// </summary>
    private readonly TimeSpan _tickInterval;

    /// <summary>
    ///     The <see cref="Timer" />.
    /// </summary>
    private Timer? _timer;

    protected TickingService(in TimeSpan tickInterval, ILogger logger)
    {
        this._tickInterval = tickInterval;
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._bufferBlock = CreateBufferBlock();
    }

    /// <summary>
    ///     The <see cref="ILogger" />.
    /// </summary>
    protected ILogger Logger { get; }

    public void Start()
    {
        // start a timer to fire the tickFunction
        this._timer = new(callback: this.QueueTick, state: null, dueTime: TimeSpan.Zero, period: Timeout.InfiniteTimeSpan);
    }

    public void Stop()
    {
        // stop the timer
        this._timer?.Dispose();
        this._timer = null;
    }

    protected abstract Task TickAsync();

    private void QueueTick(object? state)
    {
        try
        {
            this._bufferBlock.Post(this.TickAsync);
        }
        catch (Exception e)
        {
            this.Logger.LogError(new(e.HResult), exception: e, message: e.Message);
        }
        finally
        {
            // and reset the timer
            this._timer?.Change(dueTime: this._tickInterval, period: TimeSpan.Zero);
        }
    }

    private static BufferBlock<Func<Task>> CreateBufferBlock()
    {
        BufferBlock<Func<Task>> bufferBlock = new();

        // link the buffer block to an action block to process the actions submitted to the buffer.
        // restrict the number of parallel tasks executing to 1, and only allow 1 messages per task to prevent
        // tasks submitted here from consuming all the available CPU time.
        bufferBlock.LinkTo(new ActionBlock<Func<Task>>(action: action => action(), new() { MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1, BoundedCapacity = 1 }));

        return bufferBlock;
    }
}