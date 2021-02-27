using Discord.Commands;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DBot.Commands.General
{
    public class Toggle : ModuleBase<SocketCommandContext>
    {
        [Command("toggle"),
         Summary("Toggle a command on and off.")]
        public async Task ToggleCommandAsync([Required] string command, [Required] bool enabled)
        {
            DiscordBot.Database.UpdateGuildCommand(Context.Guild, command, enabled);
            await ReplyAsync($"Command `{command}` {(enabled ? "enabled" : "disabled")}");
        }
    }
}
