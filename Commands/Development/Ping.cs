using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DBot.Commands.Development
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping"),
         Summary("Pings the bot. Usage: <prefix>ping")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task PingAsync()
        {
            await ReplyAsync($"Pong. `({DiscordBot.GetLatency()}ms)`");
        }
    }
}
