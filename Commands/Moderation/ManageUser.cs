using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DBot.Commands.Moderation
{
    public class ManageUser : ModuleBase<SocketCommandContext>
    {
        [Command("kick"),
         Summary("Kicks specified user.")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUserAsync([Required] SocketUser user, string reason = "")
        {
            await (user as IGuildUser).KickAsync(reason);

            await ReplyAsync($"Kicked {user.Mention} for \"{reason}{(reason.EndsWith('.') ? string.Empty : ".")}\"");
        }

        [Command("ban"),
         Summary("Bans specified user.")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync([Required] SocketUser user, string reason = "")
        {
            await (user as IGuildUser).BanAsync(reason: reason);

            await ReplyAsync($"Banned {user.Mention} for \"{reason}{(reason.EndsWith('.') ? string.Empty : ".")}\"");
        }
    }
}
