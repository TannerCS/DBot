using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DBot
{

    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _Commands;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService Commands)
        {
            _Commands = Commands;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // See Dependency Injection guide for more information.
            await _Commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null || message.Channel is IDMChannel) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            var prefix = DiscordBot.Database.GetGuildData((message.Channel as SocketGuildChannel).Guild).Prefix;

            // Determine if the message is a command based on the prefix and make sure no bots trigger Commands
            if (!(message.HasStringPrefix(prefix, ref argPos)) || message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            var command = _Commands.Search(context, argPos);
            if (!command.IsSuccess) return;

            if (!DiscordBot.Database.IsCommandEnabledForGuild(context.Guild, command.Commands.FirstOrDefault().Command.Name))
            {
                await context.Channel.SendMessageAsync($"Command not enabled.");
                return;
            }

            if (!DiscordBot.Database.CanUserRunCommand(context.User as IGuildUser, command.Commands.FirstOrDefault().Command.Name))
                return;

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _Commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }
    }
}
