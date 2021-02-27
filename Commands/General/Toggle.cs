using Discord.Commands;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DBot.Commands.General
{
    public class Toggle : ModuleBase<SocketCommandContext>
    {
        [Command("toggle"),
         Summary("Toggle a command on and off.")]
        public async Task ToggleCommandAsync([Required] string command)
        {
            var guildData = DiscordBot.Database.GetGuildData(Context.Guild);

            if (!guildData.Commands.ContainsKey(command)) return;

            DiscordBot.Database.UpdateGuildCommand(Context.Guild, command, !guildData.Commands[command]);
            await ReplyAsync($"Command `{command}` {(guildData.Commands[command] ? "disabled" : "enabled")}");
        }
    }
}
