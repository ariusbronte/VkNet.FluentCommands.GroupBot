using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model.GroupUpdate;

namespace VkNet.FluentCommands.GroupBot.Storage
{
    internal class ReplyCommandsStore : BaseStore<(long? peerId, string pattern, RegexOptions options),
        IVkApi, MessageNew, CancellationToken, Task>
    {
        public void Store((long? peerId, string pattern, RegexOptions options) key,
            Func<IVkApi, MessageNew, CancellationToken, Task> value)
        {
            if (string.IsNullOrWhiteSpace(key.pattern))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key.pattern));
            
            if (!Enum.IsDefined(typeof(RegexOptions), key.options))
                throw new InvalidEnumArgumentException(nameof(key.options), (int) key.options, typeof(RegexOptions));

            if (value == null) throw new ArgumentNullException(nameof(value));
            
            StoreValue(key, value);
        }

        public ConcurrentDictionary<(long? peerId, string pattern, RegexOptions options),
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