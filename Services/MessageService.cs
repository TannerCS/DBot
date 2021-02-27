using Discord;
using Discord.Commands;
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
		}
		private async Task MessageReceivedAsync(SocketMessage message)
		{
			await Task.CompletedTask;
		}
	}
}