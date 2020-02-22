using VkNet.Abstractions;
using VkNet.Model.GroupUpdate;

namespace VkNet.FluentCommands.GroupBot.Abstractions
{
    internal class MessageToProcess
    {
        public IVkApi BotClient { get; }
        
        public MessageNew Message { get; }

        public MessageToProcess(IVkApi botClient, MessageNew message)
        {
            BotClient = botClient;
            Message = message;
        }
    }
}