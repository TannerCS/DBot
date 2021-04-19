using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DBot.Commands.Moderation
{
    public class ManageUser : ModuleBase<SocketCommandContext>
    {
        [Command("kick"),
         Summary("Kicks specified user. Usage: <prefix>kick <user:userid> \"(optional)reason\"")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task KickUserAsync([Required] SocketUser user, string reason = "")
        {
            if (Context.Channel is IDMChannel) return;

            await (user as IGuildUser).KickAsync(reason);

            await ReplyAsync($"Kicked {user.Mention}{(string.IsNullOrWhiteSpace(reason) ? string.Empty : $" for {reason}")}{(reason.EndsWith('.') ? string.Empty : ".")}\"");
        }

        [Command("ban"),
         Summary("Bans specified user. Usage: <prefix>ban <user:userid> \"(optional)reason\"")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task BanUserAsync([Required] SocketUser user, string reason = "")
        {
            if (Context.Channel is IDMChannel) return;

            await (user as IGuildUser).BanAsync(reason: reason);

            await ReplyAsync($"Banned {user.Mention}{(string.IsNullOrWhiteSpace(reason) ? string.Empty : $" for {reason}")}{(reason.EndsWith('.') ? string.Empty : ".")}\"");
        }

        [Command("mute"),
         Summary("Mutes specified user. Usage: <prefix>mute <user:userid> \"(optional)reason\"")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task MuteUserAsync([Required] SocketUser user, string reason = "")
        {
            if (Context.Channel is IDMChannel) return;

            var mutedRole = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "muted");

            if (mutedRole == null)
            {
                await ReplyAsync("Muted role is not set up! Make sure you have a role called 'muted'.");
                return;
            }

            await (user as IGuildUser).AddRoleAsync(mutedRole);

            await ReplyAsync($"Muted {user.Mention}{(string.IsNullOrWhiteSpace(reason) ? string.Empty : $" for {reason}{(reason.EndsWith('.') ? string.Empty : ".")}")}");
        }

        [Command("unmute"),
         Summary("Unmutes specified user. Usage: <prefix>unmute <user:userid>")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(ChannelPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task UnMuteUserAsync([Required] SocketUser user)
        {
            if (Context.Channel is IDMChannel) return;

            var mutedRole = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "muted");

            if (mutedRole == null)
            {
                await ReplyAsync("Muted role is not set up! Make sure you have a role called 'muted'.");
                return;
            }

            if(!(user as IGuildUser).RoleIds.Contains(mutedRole.Id))
            {
                await ReplyAsync("User is not muted.");
                return;
            }

            await (user as IGuildUser).RemoveRoleAsync(mutedRole);

            await ReplyAsync($"Unmuted {user.Mention}");
        }
    }
}
