using DBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DBot
{
    public class DiscordBot
    {
        public static Config Config;
        public static Database Database;
        public static LoggingService Logger;

        private static DiscordSocketClient _Client;
        private static CommandHandler _Command;
        private static CommandService _CommandService;

        public async Task MainAsync()
        {

            _Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                DefaultRetryMode = RetryMode.AlwaysRetry,
                ExclusiveBulkDelete = true,
                LogLevel = LogSeverity.Info
            });

            _CommandService = new CommandService();

            _Command = new CommandHandler(_Client, _CommandService);
            Logger = new LoggingService(_Client, _CommandService);
            new MessageService(_Client);
            Config = new Config();
            Database = new Database();

            _Client.Ready += Ready;

            await _Command.InstallCommandsAsync();

            //TODO: Secure this
            var token = Config.Data.botToken;

            await _Client.LoginAsync(TokenType.Bot, token);
            await _Client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Ready()
        {
            foreach (var guild in _Client.Guilds)
            {
                Database.InsertNewGuild(guild, _CommandService.Commands);
                Database.UpdateGuildCommands(guild, _CommandService.Commands);
                //_CommandService.GetExecutableCommandsAsync()
            }

            return Task.CompletedTask;
        }

        public static int GetLatency()
        {
            return _Client.Latency;
        }

        public static IEnumerable<CommandInfo> GetAllCommands()
        {
            return _CommandService.Commands;
        }
    }
}
