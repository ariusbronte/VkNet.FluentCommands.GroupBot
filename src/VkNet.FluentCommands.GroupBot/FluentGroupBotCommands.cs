﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.FluentCommands.GroupBot.Abstractions;
using VkNet.FluentCommands.GroupBot.Handlers;
using VkNet.FluentCommands.GroupBot.Storage;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace VkNet.FluentCommands.GroupBot
{
    /// <inheritdoc />
    public class FluentGroupBotCommands : IFluentGroupBotCommands
    {
        /// <summary>
        ///     Implementation of interaction with VK.
        /// </summary>
        private readonly IVkApi _botClient;
        private IGroupLongPollConfiguration _longPollConfiguration;
        
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private readonly Random _random = new Random();
        
        private readonly TextCommandsStore _textCommands = new TextCommandsStore();
        private readonly ReplyCommandsStore _replyCommands = new ReplyCommandsStore();
        private readonly StickerCommandsStore _stickerCommands = new StickerCommandsStore();
        private readonly ForwardCommandsStore _forwardCommands = new ForwardCommandsStore();
        
        private readonly PhotoEventStore _photoEvent = new PhotoEventStore();
        private readonly VoiceEventStore _voiceEvent = new VoiceEventStore();
        private readonly VideoEventStore _videoEvent = new VideoEventStore();
        private readonly AudioEventStore _audioEvent = new AudioEventStore();
        private readonly GeoEventStore _geoEvent = new GeoEventStore();
        private readonly PollEventStore _pollEvent = new PollEventStore();
        private readonly DocumentEventStore _documentEvent = new DocumentEventStore();

        private readonly ChatInviteUserEventStore _chatInviteUserEvent = new ChatInviteUserEventStore();
        private readonly ChatKickUserEventStore _chatKickUserEvent = new ChatKickUserEventStore();
        private readonly ChatPhotoRemoveEventStore _chatPhotoRemoveEvent = new ChatPhotoRemoveEventStore();
        private readonly ChatPhotoUpdateEventStore _chatPhotoUpdateEvent = new ChatPhotoUpdateEventStore();
        private readonly ChatPinMessageEventStore _chatPinMessageEvent = new ChatPinMessageEventStore();
        private readonly ChatTitleUpdateEventStore _chatTitleUpdateEvent = new ChatTitleUpdateEventStore();
        private readonly ChatUnpinMessageEventStore _chatUnpinMessageEvent = new ChatUnpinMessageEventStore();
        private readonly ChatInviteUserByLinkEventStore _chatInviteUserByLinkEvent = new ChatInviteUserByLinkEventStore();

        private readonly BotExceptionEventStore _botExceptionEvent = new BotExceptionEventStore();
        private readonly ExceptionEventStore _exceptionEvent = new ExceptionEventStore();

        /// <summary>
        ///      Initializes a new instance of the <see cref="VkNet.FluentCommands.GroupBot.FluentGroupBotCommands"/> class.
        /// </summary>
        /// <param name="botClient">Implementation of interaction with VK.</param>
        public FluentGroupBotCommands(Func<IVkApi> botClient)
        {
            _botClient = botClient();
        }

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="VkNet.FluentCommands.GroupBot.FluentGroupBotCommands"/> class without parameters.
        /// </summary>
        public FluentGroupBotCommands()
        {
            _botClient = new VkApi();
        }

        /// <inheritdoc />
        public async Task InitBotAsync(IApiAuthParams apiAuthParams)
        {
            if (apiAuthParams == null) throw new ArgumentNullException(nameof(apiAuthParams));

            await _botClient.AuthorizeAsync(apiAuthParams).ConfigureAwait(false);
        }
        
        /// <inheritdoc />
        public async Task InitBotAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(accessToken));
            
            await _botClient.AuthorizeAsync(new ApiAuthParams
            {
                AccessToken = accessToken
            }).ConfigureAwait(false);
        }
        
        /// <inheritdoc />
        public void ConfigureGroupLongPoll(IGroupLongPollConfiguration configuration)
        {
            _longPollConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        /// <inheritdoc />
        public void ConfigureGroupLongPoll(ulong groupId)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId));
            _longPollConfiguration = new GroupLongPollConfiguration {GroupId = groupId};
        }

        #region TextHandlers
        /// <inheritdoc />
        public void OnText(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _textCommands.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnText(string pattern, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _textCommands.Store((null, pattern, RegexOptions.None), handler);
        }
        
        /// <inheritdoc />
        public void OnText((string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _textCommands.Store((null, tuple.pattern, tuple.options), handler);
        }
        
        /// <inheritdoc />
        public void OnText((long peerId, string pattern) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            OnText((tuple.peerId, tuple.pattern, RegexOptions.None), handler);
        }
        
        /// <inheritdoc />
        public void OnText((long peerId, string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));

            _textCommands.Store(tuple, handler);
        }
        
        /// <inheritdoc />
        public void OnText(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            OnText(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }

        /// <inheritdoc />
        public void OnText(string pattern, string answer)
        {
            OnTextHandler((null, pattern, RegexOptions.None), answer);
        }
        
        /// <inheritdoc />
        public void OnText((string pattern, RegexOptions options) tuple, string answer)
        {
            OnTextHandler((null, tuple.pattern, tuple.options), answer);
        }
        
        /// <inheritdoc />
        public void OnText((long peerId, string pattern) tuple, string answer)
        {
            OnText((tuple.peerId, tuple.pattern, RegexOptions.None), answer);
        }
        
        /// <inheritdoc />
        public void OnText((long peerId, string pattern, RegexOptions options) tuple, string answer)
        { 
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));

            OnTextHandler(tuple, answer);
        }
        
        /// <inheritdoc />
        public void OnText(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnText(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answers[_random.Next(0, answers.Length)]);
            });
        }
        
        /// <inheritdoc />
        public void OnText(string pattern, params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnTextHandler((null, pattern, RegexOptions.None), answers[_random.Next(0, answers.Length)]);
        }
        
        /// <inheritdoc />
        public void OnText((string pattern, RegexOptions options) tuple, params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnTextHandler((null, tuple.pattern, tuple.options), answers[_random.Next(0, answers.Length)]);
        }
        
        /// <inheritdoc />
        public void OnText((long peerId, string pattern) tuple, params string[] answers)
        {
            OnText((tuple.peerId, tuple.pattern, RegexOptions.None), answers);
        }
        
        /// <inheritdoc />
        public void OnText((long peerId, string pattern, RegexOptions options) tuple, params string[] answers)
        {
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));

            OnTextHandler(tuple, answers[_random.Next(0, answers.Length)]);
        }
        
        /// <summary>
        ///     Common logic of abstracted text handlers.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        private void OnTextHandler((long? peerId, string pattern, RegexOptions options) tuple, string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            _textCommands.Store(tuple, async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        #endregion

        #region ReplyHandlers
        /// <inheritdoc />
        public void OnReply(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _replyCommands.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnReply(string pattern, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _replyCommands.Store((null, pattern, RegexOptions.None), handler);
        }
        
        /// <inheritdoc />
        public void OnReply((string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _replyCommands.Store((null, tuple.pattern, tuple.options), handler);
        }
        
        /// <inheritdoc />
        public void OnReply((long peerId, string pattern) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            OnReply((tuple.peerId, tuple.pattern, RegexOptions.None), handler);
        }
        
        /// <inheritdoc />
        public void OnReply((long peerId, string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));

            _replyCommands.Store(tuple, handler);
        }
        
        /// <inheritdoc />
        public void OnReply(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            OnReply(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        
        /// <inheritdoc />
        public void OnReply(string pattern, string answer)
        {
            OnReplyHandler((null, pattern, RegexOptions.None), answer);
        }
        
        /// <inheritdoc />
        public void OnReply((string pattern, RegexOptions options) tuple, string answer)
        {
            OnReplyHandler((null, tuple.pattern, tuple.options), answer);
        }
        
        /// <inheritdoc />
        public void OnReply((long peerId, string pattern) tuple, string answer)
        {
            OnReply((tuple.peerId, tuple.pattern, RegexOptions.None), answer);
        }
        
        /// <inheritdoc />
        public void OnReply((long peerId, string pattern, RegexOptions options) tuple, string answer)
        { 
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));

            OnReplyHandler(tuple, answer);
        }
        
        /// <inheritdoc />
        public void OnReply(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));

            OnReply(answers[_random.Next(0, answers.Length)]);
        } 
        
        /// <inheritdoc />
        public void OnReply(string pattern, params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnReplyHandler((null, pattern, RegexOptions.None), answers[_random.Next(0, answers.Length)]);
        }
        
        /// <inheritdoc />
        public void OnReply((string pattern, RegexOptions options) tuple, params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnReplyHandler((null, tuple.pattern, tuple.options), answers[_random.Next(0, answers.Length)]);
        }
        
        /// <inheritdoc />
        public void OnReply((long peerId, string pattern) tuple, params string[] answers)
        {
            OnReply((tuple.peerId, tuple.pattern, RegexOptions.None), answers);
        }
        
        /// <inheritdoc />
        public void OnReply((long peerId, string pattern, RegexOptions options) tuple, params string[] answers)
        {
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));

            OnReplyHandler(tuple, answers[_random.Next(0, answers.Length)]);
        }

        /// <summary>
        ///     Common logic of abstracted reply handlers.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        private void OnReplyHandler((long? peerId, string pattern, RegexOptions options) tuple, string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) 
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            _replyCommands.Store(tuple, async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        #endregion
        
        #region ForwardHandlers
        /// <inheritdoc />
        public void OnForward(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _forwardCommands.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnForward(string pattern, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _forwardCommands.Store((null, pattern, RegexOptions.None), handler);
        }
        
        /// <inheritdoc />
        public void OnForward((string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _forwardCommands.Store((null, tuple.pattern, tuple.options), handler);
        }
        
        /// <inheritdoc />
        public void OnForward((long peerId, string pattern) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            OnForward((tuple.peerId, tuple.pattern, RegexOptions.None), handler);
        }
        
        /// <inheritdoc />
        public void OnForward((long peerId, string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));

            _forwardCommands.Store(tuple, handler);
        }
        
        public void OnForward(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            OnForward(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }

        /// <inheritdoc />
        public void OnForward(string pattern, string answer)
        {
            OnForwardHandler((null, pattern, RegexOptions.None), answer);
        }
        
        /// <inheritdoc />
        public void OnForward((string pattern, RegexOptions options) tuple, string answer)
        {
            OnForwardHandler((null, tuple.pattern, tuple.options), answer);
        }
        
        /// <inheritdoc />
        public void OnForward((long peerId, string pattern) tuple, string answer)
        {
            OnForward((tuple.peerId, tuple.pattern, RegexOptions.None), answer);
        }
        
        /// <inheritdoc />
        public void OnForward((long peerId, string pattern, RegexOptions options) tuple, string answer)
        { 
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));

            OnForwardHandler(tuple, answer);
        }
        
        /// <inheritdoc />
        public void OnForward(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnForward(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answers[_random.Next(0, answers.Length)]);
            });
        }
        
        /// <inheritdoc />
        public void OnForward(string pattern, params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnForwardHandler((null, pattern, RegexOptions.None), answers[_random.Next(0, answers.Length)]);
        }
        
        /// <inheritdoc />
        public void OnForward((string pattern, RegexOptions options) tuple, params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnForwardHandler((null, tuple.pattern, tuple.options), answers[_random.Next(0, answers.Length)]);
        }
        
        /// <inheritdoc />
        public void OnForward((long peerId, string pattern) tuple, params string[] answers)
        {
            OnText((tuple.peerId, tuple.pattern, RegexOptions.None), answers);
        }
        
        /// <inheritdoc />
        public void OnForward((long peerId, string pattern, RegexOptions options) tuple, params string[] answers)
        {
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));

            OnForwardHandler(tuple, answers[_random.Next(0, answers.Length)]);
        }
        
        /// <summary>
        ///     Common logic of abstracted forward handlers.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        private void OnForwardHandler((long? peerId, string pattern, RegexOptions options) tuple, string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            _forwardCommands.Store(tuple, async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        #endregion

        #region StickerHandlers
        /// <inheritdoc />
        public void OnSticker(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _stickerCommands.SetHandler(handler);
        }

        /// <inheritdoc />
        public void OnSticker(long stickerId, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _stickerCommands.Store((null, stickerId), handler);
        }

        /// <inheritdoc />
        public void OnSticker((long peerId, long stickerId) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));
            
            _stickerCommands.Store(tuple, handler);
        }
        
        /// <inheritdoc />
        public void OnSticker(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));

            OnSticker(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        
        /// <inheritdoc />
        public void OnSticker(long stickerId, string answer)
        {
            OnStickerHandler((null, stickerId), answer);
        }

        /// <inheritdoc />
        public void OnSticker((long peerId, long stickerId) tuple, string answer)
        {
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));
            
            OnStickerHandler(tuple, answer);
        }

        /// <inheritdoc />
        public void OnSticker(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnSticker(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answers[_random.Next(0, answers.Length)]);
            });
        }

        /// <inheritdoc />
        public void OnSticker(long stickerId, params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnStickerHandler((null, stickerId), answers[_random.Next(0, answers.Length)]);
        }

        /// <inheritdoc />
        public void OnSticker((long peerId, long stickerId) tuple, params string[] answers)
        {
            if (tuple.peerId <= 0) throw new ArgumentOutOfRangeException(nameof(tuple.peerId));
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));

            OnStickerHandler(tuple, answers[_random.Next(0, answers.Length)]);
        }

        /// <summary>
        ///     Common logic of abstracted reply handlers.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the stickerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        private void OnStickerHandler((long? peerId, long stickerId) tuple, string answer)
        {            
            if (string.IsNullOrWhiteSpace(answer))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));

            _stickerCommands.Store(tuple, async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        #endregion
        
        #region PhotoHandlers
        /// <inheritdoc />
        public void OnPhoto(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _photoEvent.SetHandler(handler);
        }

        /// <inheritdoc />
        public void OnPhoto(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));

            OnPhoto(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        
        /// <inheritdoc />
        public void OnPhoto(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnPhoto(answers[_random.Next(0, answers.Length)]);
        }
        #endregion
        
        #region VoiceHandlers
        /// <inheritdoc />
        public void OnVoice(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _voiceEvent.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnVoice(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            OnVoice(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        
        /// <inheritdoc />
        public void OnVoice(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnVoice(answers[_random.Next(0, answers.Length)]);
        }
        #endregion

        #region VideoHandlers
        /// <inheritdoc />
        public void OnVideo(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _videoEvent.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnVideo(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            OnVideo(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        
        /// <inheritdoc />
        public void OnVideo(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnVideo(answers[_random.Next(0, answers.Length)]);
        }
        #endregion
        
        #region MusicHandlers
        /// <inheritdoc />
        public void OnAudio(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _audioEvent.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnAudio(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            OnAudio(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        
        /// <inheritdoc />
        public void OnAudio(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnAudio(answers[_random.Next(0, answers.Length)]);
        }
        #endregion

        #region DocumentHandlers
        /// <inheritdoc />
        public void OnDocument(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _documentEvent.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnDocument(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            OnDocument(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        
        /// <inheritdoc />
        public void OnDocument(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnDocument(answers[_random.Next(0, answers.Length)]);
        }
        #endregion
        
        #region PollHandlers
        /// <inheritdoc />
        public void OnPoll(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _pollEvent.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnPoll(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            OnPoll(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        
        /// <inheritdoc />
        public void OnPoll(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnPoll(answers[_random.Next(0, answers.Length)]);
        }
        #endregion
        
        #region GeoHandlers
        /// <inheritdoc />
        public void OnGeo(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _geoEvent.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnGeo(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(answer));
            
            OnGeo(async (api, update, token) =>
            {
                token.ThrowIfCancellationRequested();
                await SendAsync(update.Message.PeerId, answer);
            });
        }
        
        /// <inheritdoc />
        public void OnGeo(params string[] answers)
        {
            if (answers == null) throw new ArgumentNullException(nameof(answers));
            if (answers.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(answers));
            
            OnGeo(answers[_random.Next(0, answers.Length)]);
        }
        #endregion
        
        #region EventHandlers
        /// <inheritdoc />
        public void OnChatInviteUserAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _chatInviteUserEvent.SetHandler(handler);
        }

        /// <inheritdoc />
        public void OnChatKickUserAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _chatKickUserEvent.SetHandler(handler);
        }

        /// <inheritdoc />
        public void OnChatPhotoRemoveAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _chatPhotoRemoveEvent.SetHandler(handler);
        }

        /// <inheritdoc />
        public void OnChatPhotoUpdateAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _chatPhotoUpdateEvent.SetHandler(handler);
        }

        /// <inheritdoc />
        public void OnChatPinMessageAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _chatPinMessageEvent.SetHandler(handler);
        }

        /// <inheritdoc />
        public void OnChatTitleUpdateAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _chatTitleUpdateEvent.SetHandler(handler);
        }

        /// <inheritdoc />
        public void OnChatUnpinMessageAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _chatUnpinMessageEvent.SetHandler(handler);
        }

        /// <inheritdoc />
        public void OnChatInviteUserByLinkAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler)
        {
            _chatInviteUserByLinkEvent.SetHandler(handler);
        }
        #endregion
        
        #region ExceptionHandlers
        /// <inheritdoc />
        public void OnBotException(Func<IVkApi, MessageNew, System.Exception, CancellationToken, Task> handler)
        {
            _botExceptionEvent.SetHandler(handler);
        }
        
        /// <inheritdoc />
        public void OnException(Func<System.Exception, CancellationToken, Task> handler)
        {
            _exceptionEvent.SetHandler(handler);
        }
        #endregion

        /// <inheritdoc />
        public async Task ReceiveMessageAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var longPollServer = await GetLongPollServerAsync(cancellationToken).ConfigureAwait(false);

            var server = longPollServer.Server;
            var ts = longPollServer.Ts;
            var key = longPollServer.Key;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var longPollHistory = await GetBotsLongPollHistoryAsync(key, server, ts, cancellationToken).ConfigureAwait(false);
                    if (!longPollHistory.Updates.Any()) continue;

                    foreach (var update in longPollHistory.Updates)
                    {
                        try
                        {
                            if (update?.Type != GroupUpdateType.MessageNew) continue;
                            if (update?.MessageNew == null) continue;
                            var messageNew = update.MessageNew;
                            var message = messageNew.Message;
                            if (!message.PeerId.HasValue) throw new System.Exception("No PeerId");
                            if (!message.FromId.HasValue) throw new System.Exception("No FromId");

                            var forwardedMessages = message.ForwardedMessages?.ToArray() ?? new Message[] { };
                            var attachments = message.Attachments?.ToArray() ?? new Attachment[] { };
                            var replyMessage = message.ReplyMessage;
                            var actionObject = message.Action;
                            var geo = message.Geo;
                            var type = DetectMessageType(forwardedMessages, attachments, actionObject, geo, replyMessage);
                            var messageToProcess = new MessageToProcess(_botClient, messageNew);
                            switch (type)
                            {
                                case VkMessageType.Message:
                                case VkMessageType.Reply:
                                case VkMessageType.Sticker:
                                case VkMessageType.Forward:
                                    await CreateCommandHandler(type).Handle(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.Photo:
                                    await _photoEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.Voice:
                                    await _voiceEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.Video:
                                    await _videoEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.Audio:
                                    await _audioEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.Document:
                                    await _documentEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.Poll:
                                    await _pollEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.Geo:
                                    await _geoEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.ChatInviteUser:
                                    await _chatInviteUserEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.ChatKickUser:
                                    await _chatKickUserEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.ChatPhotoRemove:
                                    await _chatPhotoRemoveEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.ChatPhotoUpdate:
                                    await _chatPhotoUpdateEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.ChatPinMessage:
                                    await _chatPinMessageEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.ChatTitleUpdate:
                                    await _chatTitleUpdateEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.ChatUnpinMessage:
                                    await _chatUnpinMessageEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                case VkMessageType.ChatInviteUserByLink:
                                    await _chatInviteUserByLinkEvent.TriggerHandler(messageToProcess, cancellationToken).ConfigureAwait(false);
                                    continue;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        catch (System.Exception e)
                        {
                            var messageToProcess = new MessageToProcess(_botClient, update.MessageNew);
                            await _botExceptionEvent.TriggerHandler(messageToProcess, e, cancellationToken).ConfigureAwait(false);
                            continue;
                        }
                    }
                    
                    ts = longPollHistory.Ts;
                }
                catch (LongPollKeyExpiredException e)
                {
                    longPollServer = await GetLongPollServerAsync(cancellationToken).ConfigureAwait(false);

                    server = longPollServer.Server;
                    ts = longPollServer.Ts;
                    key = longPollServer.Key;

                    await _exceptionEvent.TriggerHandler(e, cancellationToken).ConfigureAwait(false);
                }
                catch (System.Exception e)
                {
                    await _exceptionEvent.TriggerHandler(e, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static VkMessageType DetectMessageType(
            IReadOnlyCollection<Message> forwardMessages,
            IReadOnlyCollection<Attachment> attachments,
            MessageActionObject actionObject,
            Geo geo,
            Message replyMessage)
        {
            if (actionObject != null)
            {
                if (actionObject.Type == MessageAction.ChatInviteUser) return VkMessageType.ChatInviteUser;
                if (actionObject.Type == MessageAction.ChatKickUser) return VkMessageType.ChatKickUser;
                if (actionObject.Type == MessageAction.ChatPhotoRemove) return VkMessageType.ChatPhotoRemove;
                if (actionObject.Type == MessageAction.ChatPhotoUpdate) return VkMessageType.ChatPhotoUpdate;
                if (actionObject.Type == MessageAction.ChatPinMessage) return VkMessageType.ChatPinMessage;
                if (actionObject.Type == MessageAction.ChatTitleUpdate) return VkMessageType.ChatTitleUpdate;
                if (actionObject.Type == MessageAction.ChatUnpinMessage) return VkMessageType.ChatUnpinMessage;
                if (actionObject.Type == MessageAction.ChatInviteUserByLink) return VkMessageType.ChatInviteUserByLink;
                
                throw new ArgumentException("action type not found");
            }
            
            if (forwardMessages?.Count > 0) return VkMessageType.Forward;
            if (replyMessage != null) return VkMessageType.Reply;
            if (geo != null) return VkMessageType.Geo;

            if (attachments?.Count > 0)
            {
                foreach (var attachment in attachments)
                {
                    if (attachment.Type == typeof(Sticker)) return VkMessageType.Sticker;
                    if (attachment.Type == typeof(Photo)) return VkMessageType.Photo;
                    if (attachment.Type == typeof(AudioMessage)) return VkMessageType.Voice;
                    if (attachment.Type == typeof(Video)) return VkMessageType.Video;
                    if (attachment.Type == typeof(Audio)) return VkMessageType.Audio;
                    if (attachment.Type == typeof(Document)) return VkMessageType.Document;
                    if (attachment.Type == typeof(Poll)) return VkMessageType.Poll;
                }
            }
            

            return VkMessageType.Message;
        }
        
        private ICommandHandler<MessageToProcess> CreateCommandHandler(VkMessageType vkMessageType)
        {
            if(vkMessageType == VkMessageType.Forward) return new ForwardCommandHandler(_forwardCommands);
            if(vkMessageType == VkMessageType.Reply) return new ReplyCommandHandler(_replyCommands);
            if(vkMessageType == VkMessageType.Sticker) return new StickerCommandHandler(_stickerCommands);
            
            return new TextCommandHandler(_textCommands);
        }
        
        /// <summary>
        ///     Get data for the connection
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Returns data for the connection to long poll</returns>
        private async Task<LongPollServerResponse> GetLongPollServerAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await _botClient.Groups.GetLongPollServerAsync(_longPollConfiguration.GroupId).ConfigureAwait(false);
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
            return await _botClient.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
            {
                Key = key,
                Server = server,
                Ts = ts,
                Wait = _longPollConfiguration.Wait
            }).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a private message.
        /// </summary>
        /// <param name="peerId">
        ///     The ID of the destination. For a group conversation: 2000000000 + conversation id. For
        ///     communities: - community id.
        /// </param>
        /// <param name="message">
        ///     Private message text (required if the parameter is not set attachment)
        /// </param>
        private async Task SendAsync(long? peerId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
            
            await _botClient.Messages.SendAsync(new MessagesSendParams
            {
                PeerId = peerId,
                Message = message,
                RandomId = GetRandomId()
            }).ConfigureAwait(false);
        }

        /// <summary>
        ///     Returns unique identifier, used to prevent re-sending same message.
        ///     Is saved with the message and is available in the message history.
        /// </summary>
        /// <code>
        ///    await _vkApi.Messages.SendAsync(new MessagesSendParams{ RandomId = GetRandomId() });
        /// </code>
        private int GetRandomId()
        {
            var intBytes = new byte[4];
            _rng.GetBytes(intBytes);

            return BitConverter.ToInt32(intBytes, 0);
        }
        
        /// <inheritdoc cref="IDisposable" />
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}