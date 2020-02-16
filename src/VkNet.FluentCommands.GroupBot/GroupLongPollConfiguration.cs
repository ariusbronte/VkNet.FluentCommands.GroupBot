﻿namespace VkNet.FluentCommands.GroupBot
{
    /// <summary>
    ///    Long poll configuration
    /// </summary>
    public class GroupLongPollConfiguration
    {
        /// <summary>
        ///     Group identifier
        /// </summary>
        public ulong GroupId { get; set; }

        /// <summary>
        ///     Wait time. The maximum value is 90.
        /// </summary>
        public int Wait { get; set; } = 25;
    }
}