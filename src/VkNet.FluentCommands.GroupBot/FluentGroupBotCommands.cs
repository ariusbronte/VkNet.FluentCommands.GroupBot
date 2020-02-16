using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace VkNet.FluentCommands.GroupBot
{
    /// <inheritdoc />
    public class FluentGroupBotCommands : FluentGroupBotCommands<IVkApi>
    {
        /// <summary>
        ///      Initializes a new instance of the <see cref="FluentGroupBotCommands"/> class.
        /// </summary>
        public FluentGroupBotCommands() : base(() => new VkApi())
        {
        }
    }

    /// <summary>
    ///     Main entry class to use VkNet.FluentCommands.GroupBot.
    /// </summary>
    /// <typeparam name="TBotClient">Custom implementation of interaction with VK.</typeparam>
    public class FluentGroupBotCommands<TBotClient> where TBotClient : IVkApi
    {
        /// <summary>
        ///     Implementation of interaction with VK.
        /// </summary>
        private readonly TBotClient _botClient;

        /// <summary>
        ///     Longpoll configuration
        /// </summary>
        private GroupLongPollConfiguration _longPollConfiguration;

        /// <summary>
        ///     Text commands storage.
        /// </summary>
        private readonly ConcurrentDictionary<(string, RegexOptions), Func<IVkApi, GroupUpdate, CancellationToken, Task>>
            _textCommands =
                new ConcurrentDictionary<(string, RegexOptions), Func<IVkApi, GroupUpdate, CancellationToken, Task>>();

        /// <summary>
        ///      Initializes a new instance of the <see cref="FluentGroupBotCommands{TBotClient}"/> class.
        /// </summary>
        /// <param name="botClient">Implementation of interaction with VK.</param>
        public FluentGroupBotCommands(Func<TBotClient> botClient)
        {
            _botClient = botClient();
        }

        /// <summary>
        ///     Authorize of the bot.
        /// </summary>
        /// <param name="apiAuthParams">Authorization parameter.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiAuthParams is null.</exception>
        public async Task InitBotAsync(IApiAuthParams apiAuthParams)
        {
            if (apiAuthParams == null)
            {
                throw new ArgumentNullException(nameof(apiAuthParams));
            }

            await _botClient.AuthorizeAsync(apiAuthParams);
        }

        /// <summary>
        ///     Method to set custom <see cref="VkNet.FluentCommands.GroupBot.GroupLongPollConfiguration"/>.
        /// </summary>
        /// <param name="configuration">Custom long poll configuration.</param>
        public void ConfigureGroupLongPoll(GroupLongPollConfiguration configuration)
        {
            _longPollConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        public void OnText(string pattern, Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            OnText((pattern, RegexOptions.None), func);
        }

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        public void OnText((string pattern, RegexOptions options) tuple,
            Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            if (string.IsNullOrWhiteSpace(tuple.pattern))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(tuple.pattern));
            }

            if (!Enum.IsDefined(typeof(RegexOptions), tuple.options))
            {
                throw new InvalidEnumArgumentException(nameof(tuple.options), (int) tuple.options,
                    typeof(RegexOptions));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            _textCommands.TryAdd((tuple.pattern, tuple.options), func);
        }
    }
}