using DBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
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
            _Client.JoinedGuild += JoinedGuild;
            _Client.LeftGuild += LeftGuild;
            _Client.MessageReceived += MessageReceived;
            _Client.MessageDeleted += MessageDeleted;
            _Client.UserBanned += UserBanned;
            _Client.UserJoined += UserJoined;
            _Client.UserLeft += UserLeft;
            _Client.UserUnbanned += UserUnbanned;
            //_Client.InviteCreated += InviteCreated;
            //_Client.InviteDeleted += InviteDeleted;
            //_Client.LatencyUpdated += LatencyUpdated;
            //_Client.MessagesBulkDeleted += MessageBulkDeleted;


            await _Command.InstallCommandsAsync();

            var token = Config.Data.botToken;

            await _Client.LoginAsync(TokenType.Bot, token);
            await _Client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task UserUnbanned(SocketUser arg1, SocketGuild arg2)
        {
            var guild = arg2;

            var guildData = Database.GetGuildData(guild);

            if (guildData.Analytics.UsersBanned.Count < 1)
                guildData.Analytics.UsersBanned.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            var messageReceivedAnalytic = guildData.Analytics.UsersBanned.Last();

            if (DateTime.Now.Subtract(new DateTime(messageReceivedAnalytic.Timestamp)).TotalHours > 24)
                guildData.Analytics.UsersBanned.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            messageReceivedAnalytic.Count++;
            guildData.Analytics.UsersBanned[guildData.Analytics.UsersBanned.Count - 1] = messageReceivedAnalytic;

            Database.UpdateAnalytics(guildData.Analytics, guild);
        }

        private async Task UserBanned(SocketUser arg1, SocketGuild arg2)
        {
            var guild = arg2;

            var guildData = Database.GetGuildData(guild);

            if (guildData.Analytics.UsersBanned.Count < 1)
                guildData.Analytics.UsersBanned.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            var messageReceivedAnalytic = guildData.Analytics.UsersBanned.Last();

            if (DateTime.Now.Subtract(new DateTime(messageReceivedAnalytic.Timestamp)).TotalHours > 24)
                guildData.Analytics.UsersBanned.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            messageReceivedAnalytic.Count++;
            guildData.Analytics.UsersBanned[guildData.Analytics.UsersBanned.Count - 1] = messageReceivedAnalytic;

            Database.UpdateAnalytics(guildData.Analytics, guild);
        }

        private async Task UserLeft(SocketGuildUser arg)
        {
            var guild = arg.Guild;

            var guildData = Database.GetGuildData(guild);

            if (guildData.Analytics.UsersLeft.Count < 1)
                guildData.Analytics.UsersLeft.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            var messageReceivedAnalytic = guildData.Analytics.UsersJoined.Last();

            if (DateTime.Now.Subtract(new DateTime(messageReceivedAnalytic.Timestamp)).TotalHours > 24)
                guildData.Analytics.UsersLeft.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            messageReceivedAnalytic.Count++;
            guildData.Analytics.UsersLeft[guildData.Analytics.UsersLeft.Count - 1] = messageReceivedAnalytic;
            Console.Write(guildData.Analytics.UsersLeft.Last().Count);
            Database.UpdateAnalytics(guildData.Analytics, guild);
        }

        private async Task UserJoined(SocketGuildUser arg)
        {
            var guild = arg.Guild;

            var guildData = Database.GetGuildData(guild);

            if (guildData.Analytics.UsersJoined.Count < 1)
                guildData.Analytics.UsersJoined.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            var messageReceivedAnalytic = guildData.Analytics.UsersJoined.Last();

            if (DateTime.Now.Subtract(new DateTime(messageReceivedAnalytic.Timestamp)).TotalHours > 24)
                guildData.Analytics.UsersJoined.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            messageReceivedAnalytic.Count++;
            guildData.Analytics.UsersJoined[guildData.Analytics.UsersJoined.Count - 1] = messageReceivedAnalytic;

            Database.UpdateAnalytics(guildData.Analytics, guild);
        }

        private async Task MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (arg2 is IDMChannel) return;

            var guild = (arg2 as IGuildChannel).Guild;

            var guildData = Database.GetGuildData(guild);

            if (guildData.Analytics.MessagesDeleted.Count < 1)
                guildData.Analytics.MessagesDeleted.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            var messageReceivedAnalytic = guildData.Analytics.MessagesDeleted.Last();

            if (DateTime.Now.Subtract(new DateTime(messageReceivedAnalytic.Timestamp)).TotalHours > 24)
                guildData.Analytics.MessagesDeleted.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            messageReceivedAnalytic.Count++;
            guildData.Analytics.MessagesDeleted[guildData.Analytics.MessagesDeleted.Count - 1] = messageReceivedAnalytic;

            Database.UpdateAnalytics(guildData.Analytics, guild);
        }

        private async Task MessageReceived(SocketMessage arg)
        {
            if (arg.Channel is IDMChannel) return;

            var guild = (arg.Channel as IGuildChannel).Guild;

            var guildData = Database.GetGuildData(guild);

            if(guildData.Analytics.MessagesReceived.Count < 1)
                guildData.Analytics.MessagesReceived.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            var messageReceivedAnalytic = guildData.Analytics.MessagesReceived.Last();

            if (DateTime.Now.Subtract(new DateTime(messageReceivedAnalytic.Timestamp)).TotalHours >= 24)
                guildData.Analytics.MessagesReceived.Add(new Constants.AnalyticData.Analytic() { Count = 0, Timestamp = DateTime.Now.Ticks });

            messageReceivedAnalytic = guildData.Analytics.MessagesReceived.Last();

            messageReceivedAnalytic.Count++;
            guildData.Analytics.MessagesReceived[guildData.Analytics.MessagesReceived.Count - 1] = messageReceivedAnalytic;

            Database.UpdateAnalytics(guildData.Analytics, guild);
        }

        private async Task LeftGuild(SocketGuild arg)
        {
            Database.RemoveGuildData(arg);
        }

        private async Task JoinedGuild(SocketGuild arg)
        {
            Database.InsertNewGuild(arg);
            await Database.UpdateGuildCommands(arg);
        }

        private async Task Ready()
        {
            var commandCount = CommandService.Commands.Count();
            await _Client.SetGameAsync($" for {commandCount} commands across {_Client.Guilds.Count} guilds.", null, ActivityType.Watching);

            //Check that every guild is up-to-date with commands
            foreach (var guild in _Client.Guilds)
            {
                Database.InsertNewGuild(guild);
                await Database.UpdateGuildCommands(guild);
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
