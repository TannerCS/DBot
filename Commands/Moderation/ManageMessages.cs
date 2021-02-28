using Discord.Commands;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DBot.Commands.Moderation
{
    public class ManageMessages : ModuleBase<SocketCommandContext>
    {
        [Command("delete"),
         Summary("Deletes the specified message. Usage: <prefix>delete <messageID:message reply>")]
        [RequireBotPermission(Discord.ChannelPermission.ManageMessages)]
        [RequireUserPermission(Discord.ChannelPermission.ManageMessages)]
        public async Task DeleteMessageAsync([Optional] ulong messageID)
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
