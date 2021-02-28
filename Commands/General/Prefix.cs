using Discord.Commands;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DBot.Commands.General
{
    public class Prefix : ModuleBase<SocketCommandContext>
    {
        [Command("prefix"),
         Summary("Changes the prefix for the guild.")]
        public async Task PrefixAsync([Required] string prefix)
        {
            DiscordBot.Database.ChangeGuildPrefix(Context.Guild, prefix);
            await ReplyAsync($"Changed prefix to `{prefix}`");
        }
    }
}
