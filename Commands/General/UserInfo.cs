using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DBot.Commands.General
{
    public class UserInfo : ModuleBase<SocketCommandContext>
    {
        [Command("userinfo")]
        [Summary("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfoAsync([Summary("The (optional) user to get info from")] SocketUser user = null)
        {
            var embed = new EmbedBuilder();
            var userInfo = user ?? Context.User;

            embed.WithTitle($"{userInfo.Username}#{userInfo.Discriminator}");
            embed.WithUrl($"https://discord.com/users/{userInfo.Id}/");
            embed.WithThumbnailUrl(userInfo.GetAvatarUrl(ImageFormat.Png));
            embed.WithDescription($"Account creation date:\n{userInfo.CreatedAt.UtcDateTime.ToString("F")} (UTC)");

            await ReplyAsync(embed: embed.Build());
        }
    }
}
