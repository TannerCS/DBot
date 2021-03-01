using DBot.Constants;
using Discord;
using Discord.Commands;
using System.Net;
using System.Threading.Tasks;

namespace DBot.Commands.Gaming
{
    public class SteamServer : ModuleBase<SocketCommandContext>
    {
        [Command("server")]
        [Summary("Displays information about a specific Steam server. Usage: <prefix>server <ip:port>")]
        public async Task SteamCommand(string ip)
        {
            var splitIP = ip.Split(':');

            if (splitIP.Length != 2) return;

            var parsedIP = new IPEndPoint(IPAddress.Parse(splitIP[0]), int.Parse(splitIP[1]));
            var as2Info = new A2S_INFO(parsedIP);

            if (!as2Info.IsSuccess || as2Info.Visibility == A2S_INFO.VisibilityFlags.Private) return;

            var embed = new EmbedBuilder();
            embed.WithTitle(as2Info.Game);
            embed.WithDescription($"**{as2Info.Name}**");
            embed.AddField("Players", $"{as2Info.Players}/{as2Info.MaxPlayers}", true);
            embed.AddField("Map", as2Info.Map, true);

            await ReplyAsync(embed: embed.Build());
        }
    }
}
