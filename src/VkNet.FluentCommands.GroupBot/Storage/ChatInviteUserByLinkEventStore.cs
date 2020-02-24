using System;
using System.Threading;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.FluentCommands.GroupBot.Abstractions;
using VkNet.Model.GroupUpdate;

namespace VkNet.FluentCommands.GroupBot.Storage
{
    internal class ChatInviteUserByLinkEventStore : BaseEventStore<IVkApi, MessageNew, CancellationToken, Task>
    {
        public void SetHandler(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            SetEventHandler(handler);
        }

        public async Task TriggerHandler(MessageToProcess messageToProcess, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (messageToProcess.BotClient == null) throw new ArgumentNullException(nameof(messageToProcess.BotClient));
            if (messageToProcess.Message == null) throw new ArgumentNullException(nameof(messageToProcess.Message));
            await TriggerEventHandler(messageToProcess.BotClient, messageToProcess.Message, cancellationToken).ConfigureAwait(false);
        }
    }
}