using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DBot.Commands.Moderation
{
    public class ManageMessages : ModuleBase<SocketCommandContext>
    {
        [Command("delete"),
         Summary("Deletes the specified message.")]
        [RequireBotPermission(Discord.ChannelPermission.ManageMessages)]
        [RequireUserPermission(Discord.ChannelPermission.ManageMessages)]
        public async Task DeleteMessageAsync([Optional]ulong messageID)
        {
            var referencedMessage = Context.Message.ReferencedMessage ?? (messageID != 0 ? await Context.Channel.GetMessageAsync(messageID) : null);
            if (referencedMessage == null)
            {
                await ReplyAsync("Could not find message.");
                return;
            }

            await referencedMessage.DeleteAsync();
        }
    }
}
