using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model.GroupUpdate;

namespace VkNet.FluentCommands.GroupBot.Storage
{
    internal class StickerCommandsStore : BaseStore<(long? peerId, long stickerId),
            IVkApi, MessageNew, CancellationToken, Task>
    {
        public void Store((long? peerId, long stickerId) key,
            Func<IVkApi, MessageNew, CancellationToken, Task> value)
        {
            if (key.stickerId <= 0) throw new ArgumentOutOfRangeException(nameof(key.stickerId));

            StoreValue(key, value);
        }

        public ConcurrentDictionary<(long? peerId, long stickerId),
            Func<IVkApi, MessageNew, CancellationToken, Task>> Retrieve()
        {
            return RetrieveValues();
        }
        
        public void SetHandler(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            SetEventHandler(handler);
        }

        public async Task TriggerHandler(IVkApi botClient, MessageNew message, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (botClient == null) throw new ArgumentNullException(nameof(botClient));
            if (message == null) throw new ArgumentNullException(nameof(message));
            await TriggerEventHandler(botClient, message, cancellationToken).ConfigureAwait(false);
        }
    }
}