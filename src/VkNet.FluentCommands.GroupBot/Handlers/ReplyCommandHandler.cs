using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VkNet.FluentCommands.GroupBot.Abstractions;
using VkNet.FluentCommands.GroupBot.Storage;

namespace VkNet.FluentCommands.GroupBot.Handlers
{
    internal class ReplyCommandHandler : ICommandHandler<MessageToProcess>
    {
        private readonly ReplyCommandsStore _commandsStore;

        public ReplyCommandHandler(ReplyCommandsStore commandsStore)
        {
            _commandsStore = commandsStore ?? throw new ArgumentNullException(nameof(commandsStore));
        }
        
        public async Task Handle(MessageToProcess messageToProcess, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var commands = _commandsStore.Retrieve();

            var botClient = messageToProcess.BotClient;
            var update = messageToProcess.Message;
            var message = update.Message;
            
            if (commands.IsEmpty)
            {
                await _commandsStore.TriggerHandler(botClient, update, cancellationToken);
                return;
            }

            var command = commands.AsParallel().Where(x =>
            {
                var peerId = x.Key.peerId;
                var pattern = x.Key.pattern;
                var options = x.Key.options;

                if (peerId == message.PeerId)
                {
                    return Regex.IsMatch(message.Text, pattern, options);
                }

                if (peerId.HasValue)
                {
                    return false;
                }

                return Regex.IsMatch(message.Text, pattern, options);
            }).Select(x => x.Value).FirstOrDefault();

            if (command == null)
            {
                await _commandsStore.TriggerHandler(botClient, update, cancellationToken);
                return;
            }

            await command(botClient, update, cancellationToken);
        }
    }
}