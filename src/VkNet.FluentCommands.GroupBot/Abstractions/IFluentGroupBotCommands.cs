using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

namespace VkNet.FluentCommands.GroupBot.Abstractions
{
    public interface IFluentGroupBotCommands
    {
        /// <summary>
        ///     Authorize of the bot with extended parameters.
        /// </summary>
        /// <param name="apiAuthParams">Authorization parameter.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if apiAuthParams is null.</exception>
        Task InitBotAsync(IApiAuthParams apiAuthParams);
        
        /// <summary>
        ///     Authorize of the bot with access token only.
        /// </summary>
        /// <param name="accessToken">Access token.</param>
        /// <exception cref="System.ArgumentException">Thrown if accessToken is null or whitespace.</exception>
        Task InitBotAsync(string accessToken);

        /// <summary>
        ///     Configure <see cref="VkNet.FluentCommands.GroupBot.GroupLongPollConfiguration"/> with extended parameters.
        /// </summary>
        /// <param name="configuration">Custom long poll configuration.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if configuration is null.</exception>
        void ConfigureGroupLongPoll(IGroupLongPollConfiguration configuration);

        /// <summary>
        ///     Configure <see cref="VkNet.FluentCommands.GroupBot.GroupLongPollConfiguration"/> with group id only.
        /// </summary>
        /// <param name="groupId">Group id.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the groupId is less than or equal to zero.</exception>
        void ConfigureGroupLongPoll(ulong groupId);
        
        /// <summary>
        ///     Global extended handler of all incoming messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.TextCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnText(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnText(string pattern, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming message based on the regular expression options.
        /// </summary>
        /// <param name="tuple">Regular expression with options.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnText((string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming message. Only works for an individual conversation.
        /// </summary>
        /// <param name="tuple">Regular expression with individual conversation id.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>        
        void OnText((long peerId, string pattern) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming message based on the regular expression options.
        ///     Only works for an individual conversation.
        /// </summary>
        /// <param name="tuple">Regular expression with options and individual conversation id.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnText((long peerId, string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.TextCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnText(string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnText(string pattern, string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming message based on the regular expression options.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Regular expression with options.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnText((string pattern, RegexOptions options) tuple, string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming message. Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Regular expression with individual conversation id.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnText((long peerId, string pattern) tuple, string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming message based on the regular expression options.
        ///     Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Regular expression with options and individual conversation id.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if answer in null or whitespace.</exception>
        void OnText((long peerId, string pattern, RegexOptions options) tuple, string answer);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.TextCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnText(params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnText(string pattern, params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming message based on the regular expression options.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Regular expression with options.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnText((string pattern, RegexOptions options) tuple, params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming message. Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Regular expression with individual conversation id.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnText((long peerId, string pattern) tuple, params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming message based on the regular expression options.
        ///     Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Regular expression with options and individual conversation id.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnText((long peerId, string pattern, RegexOptions options) tuple, params string[] answers);

        /// <summary>
        ///     Global extended handler of all incoming reply messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.ReplyCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnReply(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnReply(string pattern, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming reply message based on the regular expression options.
        /// </summary>
        /// <param name="tuple">Regular expression with options.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnReply((string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming reply message. Only works for an individual conversation.
        /// </summary>
        /// <param name="tuple">Regular expression with individual conversation id.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>        
        void OnReply((long peerId, string pattern) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming replpy message based on the regular expression options.
        ///     Only works for an individual conversation.
        /// </summary>
        /// <param name="tuple">Regular expression with options and individual conversation id.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnReply((long peerId, string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming reply messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.ReplyCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnReply(string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnReply(string pattern, string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming reply message based on the regular expression options.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Regular expression with options.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnReply((string pattern, RegexOptions options) tuple, string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming reply message. Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Regular expression with individual conversation id.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnReply((long peerId, string pattern) tuple, string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming reply message based on the regular expression options.
        ///     Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Regular expression with options and individual conversation id.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if answer in null or whitespace.</exception>
        void OnReply((long peerId, string pattern, RegexOptions options) tuple, string answer);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming reply messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.ReplyCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnReply(params string[] answers);
        
        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnReply(string pattern, params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming reply message based on the regular expression options.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Regular expression with options.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnReply((string pattern, RegexOptions options) tuple, params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming reply message. Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Regular expression with individual conversation id.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnReply((long peerId, string pattern) tuple, params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming reply message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming reply message based on the regular expression options.
        ///     Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Regular expression with options and individual conversation id.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnReply((long peerId, string pattern, RegexOptions options) tuple, params string[] answers);
        
        /// <summary>
        ///     Global extended handler of all forward messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.TextCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnForward(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of the incoming forward message.
        /// </summary>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnForward(string pattern, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming forward message based on the regular expression options.
        /// </summary>
        /// <param name="tuple">Regular expression with options.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnForward((string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming forward message. Only works for an individual conversation.
        /// </summary>
        /// <param name="tuple">Regular expression with individual conversation id.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>        
        void OnForward((long peerId, string pattern) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming forward message based on the regular expression options.
        ///     Only works for an individual conversation.
        /// </summary>
        /// <param name="tuple">Regular expression with options and individual conversation id.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnForward((long peerId, string pattern, RegexOptions options) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming forward messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.ForwardCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnForward(string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnForward(string pattern, string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming forward message based on the regular expression options.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Regular expression with options.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnForward((string pattern, RegexOptions options) tuple, string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming forward message. Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Regular expression with individual conversation id.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnForward((long peerId, string pattern) tuple, string answer);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming forward message based on the regular expression options.
        ///     Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Regular expression with options and individual conversation id.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if answer in null or whitespace.</exception>
        void OnForward((long peerId, string pattern, RegexOptions options) tuple, string answer);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming forward messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.ForwardCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnForward(params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnForward(string pattern, params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming forward message based on the regular expression options.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Regular expression with options.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnForward((string pattern, RegexOptions options) tuple, params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming forward message. Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Regular expression with individual conversation id.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnForward((long peerId, string pattern) tuple, params string[] answers);

        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming forward message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming forward message based on the regular expression options.
        ///     Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Regular expression with options and individual conversation id.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnForward((long peerId, string pattern, RegexOptions options) tuple, params string[] answers);

        /// <summary>
        ///     Global extended handler of all incoming sticker messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.StickerCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnSticker(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     The extended handler for the incoming sticker message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <param name="stickerId">Sticker id.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the stickerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>
        void OnSticker(long stickerId, Func<IVkApi, MessageNew, CancellationToken, Task> handler);
        
        /// <summary>
        ///     The extended handler for the incoming sticker message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming sticker message. Only works for an individual conversation.
        /// </summary>
        /// <param name="tuple">Sticker id with individual conversation id.</param>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if stickerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>        
        void OnSticker((long peerId, long stickerId) tuple, Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming sticker messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.StickerCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if stickerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnSticker(string answer);
        
        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming sticker message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="stickerId">Sticker id.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if stickerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnSticker(long stickerId, string answer);
        
        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming sticker message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming sticker message. Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="tuple">Sticker id with individual conversation id.</param>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if stickerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answer in null or whitespace.</exception>
        void OnSticker((long peerId, long stickerId) tuple, string answer);
        
        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming sticker messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.StickerCommandsStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnSticker(params string[] answers);
        
        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming sticker message.
        ///     Compares the specified regular expression with the text of the incoming message.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="stickerId">Sticker id.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if stickerId is less than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnSticker(long stickerId, params string[] answers);
        
        /// <summary>
        ///     The <c>NOT</c> extended handler for the incoming sticker message.
        ///     Compares the specified regular expression with the text of
        ///     the incoming sticker message. Only works for an individual conversation.
        /// </summary>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="tuple">Sticker id with individual conversation id.</param>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if peerId is less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if stickerId is less than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnSticker((long peerId, long stickerId) tuple, params string[] answers);

        /// <summary>
        ///     Global extended handler of all incoming photo messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.PhotoEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnPhoto(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming photo messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.PhotoEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnPhoto(string answer);
        
        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming photo messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.PhotoEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnPhoto(params string[] answers);

        /// <summary>
        ///     Global extended handler of all incoming voice messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.VoiceEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnVoice(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming voice messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.VoiceEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnVoice(string answer);
        
        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming voice messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.VoiceEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnVoice(params string[] answers);
        
        /// <summary>
        ///     Global extended handler of all incoming video messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.VideoEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnVideo(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming video messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.VideoEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnVideo(string answer);
        
        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming video messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.VideoEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnVideo(params string[] answers);
        
        /// <summary>
        ///     Global extended handler of all incoming audio messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.AudioEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnAudio(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming audio messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.AudioEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnAudio(string answer);
        
        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming audio messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.AudioEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnAudio(params string[] answers);

        /// <summary>
        ///     Global extended handler of all incoming document messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.DocumentEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnDocument(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming document messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.DocumentEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnDocument(string answer);
        
        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming document messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.DocumentEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnDocument(params string[] answers);
        
        /// <summary>
        ///     Global extended handler of all incoming poll messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.PollEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnPoll(Func<IVkApi, MessageNew, CancellationToken, Task> handler);

        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming poll messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.PollEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <param name="answer">Text response.</param>
        /// <exception cref="System.ArgumentException">Thrown if answer is null or whitespace.</exception>
        void OnPoll(string answer);
        
        /// <summary>
        ///     Global <c>NOT</c> extended handler of all incoming poll messages.
        ///     Triggered if no matches are found or missing in the <see cref="VkNet.FluentCommands.GroupBot.Storage.PollEventStore"/>.
        /// </summary>
        /// <remarks>Is not required.</remarks>
        /// <remarks>Is an abstraction over the main handler.</remarks>
        /// <remarks>Selects a random string from the array to send the message to.</remarks>
        /// <param name="answers">Text responses.</param>
        /// <exception cref="ArgumentNullException">Thrown if answers is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if answers is empty.</exception>
        void OnPoll(params string[] answers);

        /// <summary>
        ///     Triggered when user invited in chat.
        /// </summary>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnChatInviteUserAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler);
            
        /// <summary>
        ///     Triggered when user kicked from chat.
        /// </summary>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnChatKickUserAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler);
        
        /// <summary>
        ///     Triggered when photo removed from chat.
        /// </summary>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnChatPhotoRemoveAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler);
        
        /// <summary>
        ///     Triggered when photo updated in chat.
        /// </summary>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnChatPhotoUpdateAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler);
        
        /// <summary>
        ///     Triggered when message pinned in chat.
        /// </summary>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnChatPinMessageAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler);
        
        /// <summary>
        ///     Triggered when title updated in chat.
        /// </summary>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnChatTitleUpdateAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler);
        
        /// <summary>
        ///     Triggered when message unpinned in chat.
        /// </summary>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnChatUnpinMessageAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler);
        
        /// <summary>
        ///     Triggered when user invited in chat by link.
        /// </summary>
        /// <param name="handler">Handler logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler is null.</exception>
        void OnChatInviteUserByLinkAction(Func<IVkApi, MessageNew, CancellationToken, Task> handler);
        
        /// <summary>
        ///     The handler for all exceptions long poll.
        /// </summary>
        /// <param name="handler">Handler logic</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>        
        void OnBotException(Func<IVkApi, MessageNew, System.Exception, CancellationToken, Task> handler);

        /// <summary>
        ///     The handler for all exceptions.
        /// </summary>
        /// <param name="handler">Handler logic</param>
        /// <exception cref="System.ArgumentNullException">Thrown if handler in null.</exception>        
        void OnException(Func<System.Exception, CancellationToken, Task> handler);

        /// <summary>
        ///     Start receiving messages.
        /// </summary>
        Task ReceiveMessageAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc cref="IDisposable" />
        void Dispose();
    }
}