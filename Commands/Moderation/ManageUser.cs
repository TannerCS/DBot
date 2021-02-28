using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DBot.Commands.Moderation
{
    public class ManageUser : ModuleBase<SocketCommandContext>
    {
        [Command("kick"),
         Summary("Kicks specified user. Usage: <prefix>kick <user:userid> \"(optional)reason\"")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUserAsync([Required] SocketUser user, string reason = "")
        {
            await (user as IGuildUser).KickAsync(reason);

            await ReplyAsync($"Kicked {user.Mention} for \"{reason}{(reason.EndsWith('.') ? string.Empty : ".")}\"");
        }

        [Command("ban"),
         Summary("Bans specified user. Usage: <prefix>ban <user:userid> \"(optional)reason\"")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync([Required] SocketUser user, string reason = "")
        {
            await (user as IGuildUser).BanAsync(reason: reason);

            await ReplyAsync($"Banned {user.Mention} for \"{reason}{(reason.EndsWith('.') ? string.Empty : ".")}\"");
        }

        [Command("mute"),
         Summary("Mutes specified user. Usage: <prefix>mute <user:userid> \"(optional)reason\"")]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        public async Task MuteUserAsync([Required] SocketUser user, string reason = "")
        {
            var mutedRole = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "muted");
            await (user as IGuildUser).AddRoleAsync(mutedRole);

            await ReplyAsync($"Muted {user.Mention}{(string.IsNullOrWhiteSpace(reason) ? string.Empty : $" for {reason}{(reason.EndsWith('.') ? string.Empty : ".")}")}");
        }

        [Command("unmute"),
         Summary("Unmutes specified user. Usage: <prefix>unmute <user:userid> \"(optional)reason\"")]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        public async Task UnMuteUserAsync([Required] SocketUser user)
        {
            var mutedRole = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "muted");
            await (user as IGuildUser).RemoveRoleAsync(mutedRole);

            await ReplyAsync($"Unmuted {user.Mention}");
        }
    }
}
