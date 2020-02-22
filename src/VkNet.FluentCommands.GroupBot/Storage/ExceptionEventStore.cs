using System;
using System.Threading;
using System.Threading.Tasks;

namespace VkNet.FluentCommands.GroupBot.Storage
{
    internal class ExceptionEventStore : BaseEventStore<System.Exception, CancellationToken, Task>
    {
        public void SetHandler(Func<System.Exception, CancellationToken, Task> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            SetEventHandler(handler);
        }

        public async Task TriggerHandler(System.Exception exception, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            
            await TriggerEventHandler(exception, cancellationToken).ConfigureAwait(false);
        }
    }
}