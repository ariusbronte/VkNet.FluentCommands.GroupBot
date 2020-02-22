using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VkNet.FluentCommands.GroupBot.Abstractions;
using VkNet.FluentCommands.GroupBot.Storage;
using VkNet.Model.Attachments;

namespace VkNet.FluentCommands.GroupBot.Handlers
{
    internal class StickerCommandHandler : ICommandHandler<MessageToProcess>
    {
        private readonly StickerCommandsStore _commandsStore;

        public StickerCommandHandler(StickerCommandsStore commandsStore)
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
            
            var sticker = message.Attachments
                .Where(x => x.Type == typeof(Sticker))
                .Select(x => x.Instance.Id)
                .SingleOrDefault();

            if (!sticker.HasValue)
            {
                throw new ArgumentNullException(nameof(sticker));
            }
            
            var command = commands.AsParallel().Where(x =>
            {
                var peerId = x.Key.peerId;
                var stickerId = x.Key.stickerId;

                if (peerId == message.PeerId)
                {
                    return sticker.Value == stickerId;
                }

                if (peerId.HasValue)
                {
                    return false;
                }

                return sticker.Value == stickerId;
            }).Select(x => x.Value).SingleOrDefault();

            if (command == null)
            {
                await _commandsStore.TriggerHandler(botClient, update, cancellationToken);
                return;
            }

            await command(botClient, update, cancellationToken);
        }
    }
}