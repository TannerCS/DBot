using Discord.Commands;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBot.Commands.Development
{
    public class Toggle : ModuleBase<SocketCommandContext>
    {
        [Command("toggle"),
         Summary("Toggle a command on and off.")]
        public async Task ToggleCommandAsync([Required] string command)
        {
            var guildData = DiscordBot.Database.GetGuildData(Context.Guild);
            var cmd = guildData.Commands.FirstOrDefault(x => x.Name == command);

            if (cmd == null)
            {
                await ReplyAsync("Command not found");
                return;
            }

            cmd.Enabled = !cmd.Enabled;
            DiscordBot.Database.UpdateGuildCommand(Context.Guild, cmd);

            await ReplyAsync($"Command `{command}` {(cmd.Enabled ? "enabled" : "disabled")}");
        }
    }
}
