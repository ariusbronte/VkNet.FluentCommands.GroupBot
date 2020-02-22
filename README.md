# VkNet.FluentCommands.GroupBot
Extension for [VkNet](https://github.com/vknet/vk) to quickly create bots.

[![NuGet](https://img.shields.io/nuget/v/VkNet.FluentCommands.GroupBot.svg)](https://www.nuget.org/packages/VkNet.FluentCommands.GroupBot/)
[![NuGet](https://img.shields.io/nuget/dt/VkNet.FluentCommands.GroupBot.svg)](https://www.nuget.org/packages/VkNet.FluentCommands.GroupBot/)

## How to use?
### Add the package to the project
**Package Manager**
``` powershell
PM> Install-Package VkNet.FluentCommands.GroupBot
```
**.NET CLI**
``` bash
> dotnet add package VkNet.FluentCommands.GroupBot
```
``` C#
using VkNet.FluentCommands.GroupBot;

//...

FluentGroupBotCommands commands = new FluentGroupBotCommands();

commands.ConfigureGroupLongPoll(000000U);

await commands.InitBotAsync("very big group token");

commands.OnText("^ping$", "pong");
commands.OnText("^hello$", new[] {"hi!", "hey!", "good day!"});
commands.OnText("command not found");

await commands.ReceiveMessageAsync();
```
``` C#
commands.OnSticker("sticker triggered");
commands.OnPhoto("photo triggered");
commands.OnVoice("voice triggered");
commands.OnSticker(163, "orejas");
commands.OnReply("^ping$", "reply pong");
commands.OnForward("^ping$, "forward pong");
```
## Extended logic
``` C# 
commands.OnText("^ping$", async (api, message, token) =>
{
    await api.Messages.SendAsync(new MessagesSendParams
    {
        PeerId = message.PeerId,
        Message = "pong",
        RandomId = random.Next(int.MinValue, int.MaxValue)
    });
});
```
*this applies to all triggers
## Regular expression configuration
``` C#
commands.OnText(("^ping$", RegexOptions.IgnoreCase), async (api, update, token) => {});
```
## Individual logic
``` C#
commands.OnText((2_000_000_000 + 1, "^ping$", RegexOptions.IgnoreCase), "pong");
commands.OnText((2_000_000_000, "^ping$", RegexOptions.IgnoreCase), async (api, update, token) => {});
```
## Bot exception handler
``` C#
commands.OnBotException(async (api, update, e, token) => {});
```

## Library exception handler
``` C#
commands.OnException((e, token) =>
{
    Console.WriteLine(e.Message);
    return Task.CompletedTask;
});
```
## Extended configurations
``` C#
commands.ConfigureGroupLongPoll(new GroupLongPollConfiguration {
    GroupId = 00000,
    Wait = 25
});

await commands.InitBotAsync(new ApiAuthParams
{
    AccessToken = ""
});
```
