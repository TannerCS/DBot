using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DBot.Services
{
    public class MessageService
    {
        public MessageService(DiscordSocketClient client)
        {
            client.MessageReceived += MessageReceivedAsync;
            client.MessageDeleted += MessageDeletedAsync;
        }

        private async Task MessageDeletedAsync(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            await Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            await Task.CompletedTask;
        }
    }
}