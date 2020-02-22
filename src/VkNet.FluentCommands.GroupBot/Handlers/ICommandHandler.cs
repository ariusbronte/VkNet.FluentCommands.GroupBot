using System.Threading;
using System.Threading.Tasks;

namespace VkNet.FluentCommands.GroupBot.Handlers
{
    internal interface ICommandHandler<in TCommandPayload>
    {
        Task Handle(TCommandPayload payload, CancellationToken cancellationToken = default);
    }
}