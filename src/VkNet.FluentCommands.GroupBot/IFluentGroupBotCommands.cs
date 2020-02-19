using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

namespace VkNet.FluentCommands.GroupBot
{
    /// <summary>
    ///     Main entry class to use VkNet.FluentCommands.GroupBot.
    /// </summary>
    /// <typeparam name="TBotClient">Custom implementation of interaction with VK.</typeparam>
    public interface IFluentGroupBotCommands : IDisposable
    {
        /// <summary>
        ///     Authorize of the bot.
        /// </summary>
        /// <param name="apiAuthParams">Authorization parameter.</param>
        /// <exception cref="ArgumentNullException">Thrown if apiAuthParams is null.</exception>
        Task InitBotAsync(IApiAuthParams apiAuthParams);

        /// <summary>
        ///     Method to set custom <see cref="VkNet.FluentCommands.GroupBot.GroupLongPollConfiguration"/>.
        /// </summary>
        /// <param name="configuration">Custom long poll configuration.</param>
        void ConfigureGroupLongPoll(GroupLongPollConfiguration configuration);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText(string pattern, Func<IVkApi, GroupUpdate, CancellationToken, Task> func);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText((string pattern, RegexOptions options) tuple, Func<IVkApi, GroupUpdate, CancellationToken, Task> func);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if sticker id is less than or equal to zero.</exception>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText((long peerId, string pattern) tuple, Func<IVkApi, GroupUpdate, CancellationToken, Task> func);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText((long peerId, string pattern, RegexOptions options) tuple, Func<IVkApi, GroupUpdate, CancellationToken, Task> func);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="answer">Short response to the received message.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if answer expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText(string pattern, string answer);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answer">Short response to the received message.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if answer expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText((string pattern, RegexOptions options) tuple, string answer);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answer">Short response to the received message.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if answer expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText((long peerId, string pattern) tuple, string answer);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answer">Short response to the received message.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if answer expression is null or whitespace.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText((long peerId, string pattern, RegexOptions options) tuple, string answer);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="pattern">Regular expression.</param>
        /// <param name="answers">Random short response to the received message.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if answer expression is null or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if sticker id is less than or equal to zero.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText(string pattern, params string[] answers);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answers">Random short response to the received message.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if answer expression is null or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if sticker id is less than or equal to zero.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText((string pattern, RegexOptions options) tuple, params string[] answers);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answers">Random short response to the received message.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if answer expression is null or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if sticker id is less than or equal to zero.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText((long peerId, string pattern) tuple, params string[] answers);

        /// <summary>
        ///     Trigger on a text command.
        /// </summary>
        /// <param name="tuple">Regular expression and Regex options.</param>
        /// <param name="answers">Random short response to the received message.</param>
        /// <exception cref="ArgumentException">Thrown if regular expression is null or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown if answer expression is null or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if sticker id is less than or equal to zero.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if regex options is not defined.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnText((long peerId, string pattern, RegexOptions options) tuple, params string[] answers);
        
        /// <summary>
        ///     Trigger on a photo command.
        /// </summary>
        /// <param name="func">Trigger actions performed.</param>
        void OnPhoto(Func<IVkApi, GroupUpdate, CancellationToken, Task> func);
        
        /// <summary>
        ///     Trigger on a sticker command.
        /// </summary>
        /// <param name="stickerId">A unique identifier sticker.</param>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if sticker id is less than or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnSticker(long stickerId, Func<IVkApi, GroupUpdate, CancellationToken, Task> func);
        
        /// <summary>
        ///     Trigger on a sticker command.
        /// </summary>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnSticker(Func<IVkApi, GroupUpdate, CancellationToken, Task> func);

        /// <summary>
        ///     Trigger on a voice command.
        /// </summary>
        /// <param name="func">Trigger actions performed.</param>
        /// <exception cref="ArgumentNullException">Thrown if trigger actions in null.</exception>
        void OnVoice(Func<IVkApi, GroupUpdate, CancellationToken, Task> func);
        
        /// <summary>
        ///     The trigger for the exception handling logic of the message.
        /// </summary>
        /// <param name="botException">Trigger actions performed.</param>
        void OnBotException(Func<IVkApi, GroupUpdate, System.Exception, CancellationToken, Task> botException);

        /// <summary>
        ///     The trigger for Library exception handling.
        /// </summary>
        /// <param name="exception">Trigger actions performed.</param>
        void OnException(Func<System.Exception, CancellationToken, Task> exception);

        /// <summary>
        ///     Starts receiving messages.
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        Task ReceiveMessageAsync(CancellationToken cancellationToken = default);
    }
}