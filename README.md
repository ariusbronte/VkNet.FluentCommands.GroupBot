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

commands.ConfigureGroupLongPoll(new GroupLongPollConfiguration
{
    GroupId = 000000
});

await commands.InitBotAsync(new ApiAuthParams
{
    AccessToken = "very big group token"
});

commands.OnText("^hello$", async (api, update, token) =>
{
    await api.Messages.SendAsync(new MessagesSendParams
    {
        PeerId = update.MessageNew.Message.PeerId,
        Message = "hi!",
        RandomId = random.Next(int.MinValue, int.MaxValue)
    });
});

await commands.ReceiveMessageAsync();
```
``` C#
commands.OnSticker(163, async (api, update, token) => {});
```
## Regular expression configuration
``` C#
commands.OnText(("^hello$", RegexOptions.IgnoreCase), async (api, update, token) => {});
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
