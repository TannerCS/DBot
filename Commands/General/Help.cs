using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DBot.Commands.General
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("Displays help info about commands. Usage: <prefix>help")]
        public async Task HelpCommand()
        {
            var commands = DiscordBot.CommandService.Commands;
            var embed = new EmbedBuilder();

            embed.WithTitle("Commands");
            foreach (var command in commands)
                embed.AddField(command.Name, command.Summary, true);

            await ReplyAsync(embed: embed.Build());
        }
    }
}
