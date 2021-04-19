using Discord;
using Discord.Commands;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DBot.Commands.General
{
    public class Prefix : ModuleBase<SocketCommandContext>
    {
        [Command("prefix"),
         Summary("Changes the prefix for the guild. Usage: <prefix>prefix <prefix>")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task PrefixAsync([Required] string prefix)
        {
            if (Context.Channel is IDMChannel) return;

            DiscordBot.Database.ChangeGuildPrefix(Context.Guild, prefix);
            await ReplyAsync($"Changed prefix to `{prefix}`");
        }
    }
}
