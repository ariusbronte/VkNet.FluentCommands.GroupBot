﻿using VkNet.FluentCommands.GroupBot.Abstractions;

 namespace VkNet.FluentCommands.GroupBot
{
    /// <inheritdoc />
    public class GroupLongPollConfiguration : IGroupLongPollConfiguration
    {
        /// <inheritdoc />
        public ulong GroupId { get; set; }

        /// <inheritdoc />
        public int Wait { get; set; } = 25;
    }
}