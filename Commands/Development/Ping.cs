using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace DBot.Commands.Development
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping"),
         Summary("Pings the bot.")]
        public Task PingAsync()
            => ReplyAsync($"Pong. `({DiscordBot.GetLatency()}ms)`");
    }
}
