using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

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
        public FluentGroupBotCommands() : base(botClient: () => new VkApi())
        {
        }
    }

    /// <inheritdoc />
    public class FluentGroupBotCommands<TBotClient> : IFluentGroupBotCommands where TBotClient : IVkApi
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
            _textCommands = new ConcurrentDictionary<(string, RegexOptions), Func<IVkApi, GroupUpdate, CancellationToken, Task>>();

        /// <summary>
        ///     Stores the message logic exception handler
        /// </summary>
        private Func<IVkApi, GroupUpdate, System.Exception, CancellationToken, Task> _botException;

        /// <summary>
        ///     Stores the library exception handler.
        /// </summary>
        private Func<System.Exception, CancellationToken, Task> _exception;

        /// <summary>
        ///      Initializes a new instance of the <see cref="FluentGroupBotCommands{TBotClient}"/> class.
        /// </summary>
        /// <param name="botClient">Implementation of interaction with VK.</param>
        public FluentGroupBotCommands(Func<TBotClient> botClient)
        {
            _botClient = botClient();
        }

        /// <inheritdoc />
        public async Task InitBotAsync(IApiAuthParams apiAuthParams)
        {
            if (apiAuthParams == null)
            {
                throw new ArgumentNullException(paramName: nameof(apiAuthParams));
            }

            await _botClient.AuthorizeAsync(@params: apiAuthParams);
        }

        /// <inheritdoc />
        public void ConfigureGroupLongPoll(GroupLongPollConfiguration configuration)
        {
            _longPollConfiguration = configuration ?? throw new ArgumentNullException(paramName: nameof(configuration));
        }

        /// <inheritdoc />
        public void OnText(string pattern, Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            OnText(tuple: (pattern, RegexOptions.None), func: func);
        }

        /// <inheritdoc />
        public void OnText((string pattern, RegexOptions options) tuple,
            Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            if (string.IsNullOrWhiteSpace(value: tuple.pattern))
            {
                throw new ArgumentException(message: "Value cannot be null or whitespace.", paramName: nameof(tuple.pattern));
            }

            if (!Enum.IsDefined(enumType: typeof(RegexOptions), value: tuple.options))
            {
                throw new InvalidEnumArgumentException(argumentName: nameof(tuple.options), invalidValue: (int) tuple.options, enumClass: typeof(RegexOptions));
            }

            if (func == null)
            {
                throw new ArgumentNullException(paramName: nameof(func));
            }

            _textCommands.TryAdd(key: (tuple.pattern, tuple.options), value: func);
        }

        /// <inheritdoc />
        public void OnBotException(Func<IVkApi, GroupUpdate, System.Exception, CancellationToken, Task> botException)
        {
            _botException = botException ?? throw new ArgumentNullException(nameof(botException));
        }

        /// <inheritdoc />
        public void OnException(Func<System.Exception, CancellationToken, Task> exception)
        {
            _exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        /// <inheritdoc />
        public async Task ReceiveMessageAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var longPollServer = await GetLongPollServerAsync(cancellationToken: cancellationToken);

            var server = longPollServer.Server;
            var ts = longPollServer.Ts;
            var key = longPollServer.Key;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var longPollHistory = await GetBotsLongPollHistoryAsync(key: key, server: server, ts: ts, cancellationToken: cancellationToken);
                    if (longPollHistory?.Updates == null)
                    {
                        continue;
                    }

                    foreach (var update in longPollHistory.Updates)
                    {
                        try
                        {
                            if (update.Type == GroupUpdateType.MessageNew)
                            {
                                var command = _textCommands
                                    .Where(predicate: x => Regex.IsMatch(input: update.MessageNew.Message.Text,
                                        pattern: x.Key.Item1, options: x.Key.Item2))
                                    .Select(selector: x => x.Value)
                                    .SingleOrDefault();

                                if (command == null)
                                {
                                    continue;
                                }

                                await command(arg1: _botClient, arg2: update, arg3: cancellationToken);
                            }
                        }
                        catch (System.Exception e)
                        {
                            await (_botException?.Invoke(_botClient, update, e, cancellationToken) ?? throw e);
                        }
                    }

                    ts = longPollHistory.Ts;
                }
                catch (LongPollKeyExpiredException e)
                {
                    longPollServer = await GetLongPollServerAsync(cancellationToken: cancellationToken);

                    server = longPollServer.Server;
                    ts = longPollServer.Ts;
                    key = longPollServer.Key;

                    if (_exception != null)
                    {
                        await _exception.Invoke(e, cancellationToken);
                    }
                }
                catch (System.Exception e)
                {
                    await (_exception?.Invoke(e, cancellationToken) ?? throw e);
                }
            }
        }

        /// <summary>
        ///     Get data for the connection
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Returns data for the connection to long poll</returns>
        private async Task<LongPollServerResponse> GetLongPollServerAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _botClient.Groups.GetLongPollServerAsync(groupId: _longPollConfiguration.GroupId);
        }

        /// <summary>
        ///     Get group events.
        /// </summary>
        /// <param name="key">A secret session key.</param>
        /// <param name="server">Server address.</param>
        /// <param name="ts">the number of the last event to start receiving data from.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Returns group events.</returns>
        private async Task<BotsLongPollHistoryResponse> GetBotsLongPollHistoryAsync(
            string key,
            string server,
            string ts,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _botClient.Groups.GetBotsLongPollHistoryAsync(@params: new BotsLongPollHistoryParams
            {
                Key = key,
                Server = server,
                Ts = ts,
                Wait = _longPollConfiguration.Wait
            });
        }

        /// <inheritdoc cref="IDisposable" />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}