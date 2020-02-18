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
using VkNet.Model.Attachments;
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
        private readonly ConcurrentDictionary<(long?, string, RegexOptions), Func<IVkApi, GroupUpdate, CancellationToken, Task>>
            _textCommands = new ConcurrentDictionary<(long?, string, RegexOptions), Func<IVkApi, GroupUpdate, CancellationToken, Task>>();

        /// <summary>
        ///     Sticker commands storage.
        /// </summary>
        private readonly ConcurrentDictionary<long, Func<IVkApi, GroupUpdate, CancellationToken, Task>>
            _stickerCommands = new ConcurrentDictionary<long, Func<IVkApi, GroupUpdate, CancellationToken, Task>>();

        /// <summary>
        ///     Stores the sticker logic handler
        /// </summary>
        private Func<IVkApi, GroupUpdate, CancellationToken, Task> _onStickerCommand;

        /// <summary>
        ///     Stores the photo logic handler
        /// </summary>
        private Func<IVkApi, GroupUpdate, CancellationToken, Task> _onPhotoCommand;
        
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
        public void OnText((string pattern, RegexOptions options) tuple, Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            OnTextHandler(tuple: (null, tuple.pattern, tuple.options), func: func);
        }
        
        /// <inheritdoc />
        public void OnText((long peerId, string pattern) tuple, Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            OnText(tuple: (tuple.peerId, tuple.pattern, RegexOptions.None), func: func);
        }
        
        /// <inheritdoc />
        public void OnText((long peerId, string pattern, RegexOptions options) tuple, Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            if (tuple.peerId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tuple.peerId));
            }
            
            OnTextHandler(tuple: tuple, func: func);
        }

        /// <summary>
        ///     The main handler for all incoming message triggers
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        private void OnTextHandler((long? peerId, string pattern, RegexOptions options) tuple, Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
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

            _textCommands.TryAdd(key: (tuple.peerId, tuple.pattern, tuple.options), value: func);
        }

        /// <inheritdoc />
        public void OnSticker(long stickerId, Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            if (stickerId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stickerId));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            _stickerCommands.TryAdd(stickerId, func);
        }

        /// <inheritdoc />
        public void OnSticker(Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            _onStickerCommand = func ?? throw new ArgumentNullException(nameof(func));
        }
        
        /// <inheritdoc />
        public void OnPhoto(Func<IVkApi, GroupUpdate, CancellationToken, Task> func)
        {
            _onPhotoCommand = func ?? throw new ArgumentNullException(nameof(func));
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
                            if (update.Type != GroupUpdateType.MessageNew) continue;

                            var type = GetMessageType(update.MessageNew.Message);
                            switch (type)
                            {
                                case MessageType.Message:
                                    await OnTextMessage(update, cancellationToken);
                                    break;
                                case MessageType.Sticker:
                                    await OnStickerMessage(update, cancellationToken);
                                    break;
                                case MessageType.Photo:
                                    if (_onPhotoCommand != null)
                                    {
                                        await _onPhotoCommand(_botClient, update, cancellationToken);
                                    }
                                    break;
                                case MessageType.None:
                                    break;
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
        ///     This method returns the type of incoming message.
        /// </summary>
        /// <param name="message">Private message.</param>
        /// <returns>The type of incoming message.</returns>
        private static MessageType GetMessageType(Message message)
        {
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                return MessageType.Message;
            }

            if (message.Attachments.Any(x => x.Type == typeof(Sticker)))
            {
                return MessageType.Sticker;
            }

            if (message.Attachments.Any(x => x.Type == typeof(Photo)))
            {
                return MessageType.Photo;
            }

            return MessageType.None;
        }

        /// <summary>
        ///     This method has the logic of processing a new message.
        /// </summary>
        /// <param name="update">Group updates</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        private async Task OnTextMessage(GroupUpdate update, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var command = _textCommands
                .AsParallel()
                .Where(x =>
                {
                    var peerId = x.Key.Item1;
                    var pattern = x.Key.Item2;
                    var options = x.Key.Item3;
                    var message = update.MessageNew.Message;

                    if (peerId == message.PeerId)
                    {
                        return Regex.IsMatch(message.Text, pattern, options);
                    }
                    
                    return !peerId.HasValue && Regex.IsMatch(message.Text, pattern, options);
                })
                .Select(x => x.Value)
                .FirstOrDefault();

            if (command == null)
            {
                return;
            }

            await command(_botClient, update, cancellationToken);
        }

        /// <summary>
        ///     This method has a sticker processing logic.
        /// </summary>
        /// <param name="update">Group updates</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        private async Task OnStickerMessage(GroupUpdate update, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stickerId = update.MessageNew.Message.Attachments
                .Where(x => x.Type == typeof(Sticker))
                .Select(x => x.Instance.Id)
                .FirstOrDefault();

            var command = _stickerCommands
                .AsParallel()
                .Where(x => x.Key == stickerId)
                .Select(x => x.Value)
                .SingleOrDefault();

            if (command == null)
            {
                if (_onStickerCommand != null)
                {
                    await _onStickerCommand(_botClient, update, cancellationToken);
                }
                
                return;
            }

            await command(_botClient, update, cancellationToken);
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