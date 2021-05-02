using DBot.Constants;
using DBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            _Client.InviteCreated += InviteCreated;
            _Client.InviteDeleted += InviteDeleted;
            _Client.LatencyUpdated += LatencyUpdated;


            await _Command.InstallCommandsAsync();

            var token = Config.Data.botToken;

            await _Client.LoginAsync(TokenType.Bot, token);
            await _Client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task LatencyUpdated(int arg1, int arg2)
        {
            await Database.UpdateLatency(arg2);
        }

        private async Task InviteDeleted(SocketGuildChannel arg1, string arg2)
        {
            var guildData = Database.GetGuildData(arg1.Guild);

            if (guildData.Analytics.InvitesDeleted.Count < 1)
                guildData.Analytics.InvitesDeleted.Add(new AnalyticData.Analytic());

            //Get unix timestamp analytic
            var inviteDeleted = guildData.Analytics.InvitesDeleted.Last();

            inviteDeleted.Count++;
            guildData.Analytics.InvitesDeleted[guildData.Analytics.UsersBanned.Count - 1] = inviteDeleted;

            Database.UpdateAnalytics(guildData.Analytics, arg1.Guild);
        }

        private async Task InviteCreated(SocketInvite arg)
        {
            var guildData = Database.GetGuildData(arg.Guild);

            if (guildData.Analytics.InvitesCreated.Count < 1)
                guildData.Analytics.InvitesCreated.Add(new AnalyticData.Analytic());

            //Get unix timestamp analytic
            var inviteCreated = guildData.Analytics.InvitesCreated.Last();

            inviteCreated.Count++;
            guildData.Analytics.InvitesCreated[guildData.Analytics.UsersBanned.Count - 1] = inviteCreated;

            Database.UpdateAnalytics(guildData.Analytics, arg.Guild);
        }

        private async Task UserUnbanned(SocketUser arg1, SocketGuild arg2)
        {
            var guildData = Database.GetGuildData(arg2);

            if (guildData.Analytics.UsersUnbanned.Count < 1)
                guildData.Analytics.UsersUnbanned.Add(new AnalyticData.Analytic());

            //Get unix timestamp analytic
            var userUnbanned = guildData.Analytics.UsersUnbanned.Last();

            userUnbanned.Count++;
            guildData.Analytics.UsersUnbanned[guildData.Analytics.UsersUnbanned.Count - 1] = userUnbanned;

            Database.UpdateAnalytics(guildData.Analytics, arg2);
        }

        private async Task UserBanned(SocketUser arg1, SocketGuild arg2)
        {
            var guildData = Database.GetGuildData(arg2);

            if (guildData.Analytics.UsersBanned.Count < 1) guildData.Analytics.UsersBanned.Add(new Constants.AnalyticData.Analytic());

            //Get unix timestamp analytic
            var userBannedAnalytic = guildData.Analytics.UsersBanned.Last();

            userBannedAnalytic.Count++;
            guildData.Analytics.UsersBanned[guildData.Analytics.UsersBanned.Count - 1] = userBannedAnalytic;

            Database.UpdateAnalytics(guildData.Analytics, arg2);
        }

        private async Task UserLeft(SocketGuildUser arg)
        {
            var guildData = Database.GetGuildData(arg.Guild);

            if (guildData.Analytics.UsersLeft.Count < 1) guildData.Analytics.UsersLeft.Add(new AnalyticData.Analytic());

            //Get unix timestamp analytic
            var userLeftAnalytic = guildData.Analytics.UsersLeft.Last();

            userLeftAnalytic.Count++;
            guildData.Analytics.UsersLeft[guildData.Analytics.UsersLeft.Count - 1] = userLeftAnalytic;
            Console.Write(guildData.Analytics.UsersLeft.Last().Count);
            Database.UpdateAnalytics(guildData.Analytics, arg.Guild);
        }

        private async Task UserJoined(SocketGuildUser arg)
        {
            var guildData = Database.GetGuildData(arg.Guild);
            if (guildData == null) return;

            if (guildData.Analytics.UsersJoined.Count < 1) guildData.Analytics.UsersJoined.Add(new AnalyticData.Analytic());

            //Get unix timestamp analytic
            var usersJoinedAnalytic = guildData.Analytics.UsersJoined.Last();

            usersJoinedAnalytic.Count++;
            guildData.Analytics.UsersJoined[guildData.Analytics.UsersJoined.Count - 1] = usersJoinedAnalytic;

            Database.UpdateAnalytics(guildData.Analytics, arg.Guild);
        }

        private async Task MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            //If message isn't from a guild, don't count it.
            if (arg2 is IDMChannel) return;
            IGuild guild = (arg2 as IGuildChannel).Guild;

            GuildData guildData = Database.GetGuildData(guild);
            if (guildData == null) return;

            if (guildData.Analytics.MessagesDeleted.Count < 1) guildData.Analytics.MessagesDeleted.Add(new AnalyticData.Analytic());

            //Get unix timestamp analytic
            var messageDeletedAnalytic = guildData.Analytics.MessagesDeleted.Last();

            messageDeletedAnalytic.Count++;
            guildData.Analytics.MessagesDeleted[guildData.Analytics.MessagesDeleted.Count - 1] = messageDeletedAnalytic;

            Database.UpdateAnalytics(guildData.Analytics, guild);
        }

        private async Task MessageReceived(SocketMessage arg)
        {
            //If message isn't from a guild, don't count it.
            if (arg.Channel is IDMChannel) return;
            IGuild guild = (arg.Channel as IGuildChannel).Guild;

            GuildData guildData = Database.GetGuildData(guild);
            if (guildData == null) return;

            if (guildData.Analytics.MessagesReceived.Count < 1) guildData.Analytics.MessagesReceived.Add(new AnalyticData.Analytic());

            //Get unix timestamp analytic
            var messageReceivedAnalytic = guildData.Analytics.MessagesReceived.Last();

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

            //Start Database update loop
            Thread worker = new Thread(new ThreadStart(Database.UpdateGuildInfoLoop));
            worker.Start();
        }

        public void UpdateGuildInfo(Database database)
        {

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
