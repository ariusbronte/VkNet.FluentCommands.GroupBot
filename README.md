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

commands.OnException((e, token) =>
{
    Console.WriteLine("Wake up, everything is broken");
    Console.WriteLine($"[{DateTime.UtcNow}] {e.Message} {Environment.NewLine} {e.StackTrace}");
    return Task.CompletedTask;
});

await commands.ReceiveMessageAsync();
```

See the [wiki](https://github.com/ariusbronte/VkNet.FluentCommands.GroupBot/wiki) for all features.
