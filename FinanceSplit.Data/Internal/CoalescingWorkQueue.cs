using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace FinanceSplit.Data.Internal;

internal sealed class CoalescingWorkQueue<TKey>
    where TKey : notnull
{
    private readonly Channel<TKey> _channel = Channel.CreateUnbounded<TKey>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false }
    );
    private readonly HashSet<TKey> _pending = [];
    private readonly HashSet<TKey> _running = [];
    private readonly HashSet<TKey> _rerun = [];
    private readonly Lock _lock = new();

    public bool TryEnqueue(TKey key)
    {
        bool written;
        lock (_lock)
        {
            if (_running.Contains(key))
            {
                return _rerun.Add(key);
            }
            else
            {
                written = _pending.Add(key);
            }
        }

        if (written)
        {
            _channel.Writer.TryWrite(key);
        }

        return written;
    }

    public async IAsyncEnumerable<TKey> ReadAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await foreach (var key in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            lock (_lock)
            {
                _pending.Remove(key);
                _running.Add(key);
            }

            yield return key;
        }
    }

    public void Complete() => _channel.Writer.TryComplete();

    internal void MarkProcessed(TKey key)
    {
        bool rerun;
        lock (_lock)
        {
            _running.Remove(key);
            rerun = _rerun.Remove(key) && _pending.Add(key);
        }

        if (rerun)
        {
            _channel.Writer.TryWrite(key);
        }
    }
}
