namespace VkNet.FluentCommands.GroupBot.Abstractions
{
    /// <summary>
    ///    Long poll configuration
    /// </summary>
    public interface IGroupLongPollConfiguration
    {
        /// <summary>
        ///     Group identifier
        /// </summary>
        ulong GroupId { get; set; }

        /// <summary>
        ///     Wait time. The maximum value is 90.
        /// </summary>
        int Wait { get; set; }
    }
}