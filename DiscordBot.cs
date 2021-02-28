using DBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBot
{
    public class DiscordBot
    {
        public static Config Config;
        public static Database Database;
        public static LoggingService Logger;
        public static CommandService CommandService;

        private static DiscordSocketClient _Client;
        private static CommandHandler _Command;

        public async Task MainAsync()
        {

            _Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                DefaultRetryMode = RetryMode.AlwaysRetry,
                ExclusiveBulkDelete = true,
                LogLevel = LogSeverity.Verbose
            });

            CommandService = new CommandService();

            _Command = new CommandHandler(_Client, CommandService);
            Logger = new LoggingService(_Client, CommandService);
            new MessageService(_Client);
            Config = new Config();
            Database = new Database();

            _Client.Ready += Ready;

            await _Command.InstallCommandsAsync();

            var token = Config.Data.botToken;

            await _Client.LoginAsync(TokenType.Bot, token);
            await _Client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task Ready()
        {
            var commandCount = CommandService.Commands.Count();
            await _Client.SetGameAsync($" for {commandCount} commands across {_Client.Guilds.Count} guilds.", null, ActivityType.Watching);

            //Check that every guild is up-to-date with commands
            foreach (var guild in _Client.Guilds)
            {
                Database.InsertNewGuild(guild);
                Database.UpdateGuildCommands(guild);
            }
        }

        public static int GetLatency()
        {
            return _Client.Latency;
        }

        public static IEnumerable<CommandInfo> GetAllCommands()
        {
            return CommandService.Commands;
        }
    }
}
