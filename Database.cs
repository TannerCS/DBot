using DBot.Constants;
using Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DBot
{
    public class Database
    {
        private MongoClient _DBClient;
        private IMongoDatabase _Database;
        private IMongoCollection<BsonDocument> _GuildInformation;
        private IMongoCollection<BsonDocument> _LatencyInformation;
        private DateTime _LastMemberUpdate = DateTime.Now;
        private static DateTime _Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public Database()
        {
            _DBClient = new MongoClient(DiscordBot.Config.Data.mongoUrl);

            _Database = _DBClient.GetDatabase("discord-bot");
            _LatencyInformation = _Database.GetCollection<BsonDocument>("bot-latency");
            _GuildInformation = _Database.GetCollection<BsonDocument>("guild-information");
        }

        public bool IsCommandEnabledForGuild(IGuild guild, string commandName)
        {
            var guildData = GetGuildData(guild);

            if (guildData == null)
                return false;

            var cmd = guildData.Commands.FirstOrDefault(x => x.Name == commandName);

            if (cmd == null)
                return false;

            return cmd.Enabled;
        }

        public bool CanUserRunCommand(IGuildUser user, string command)
        {
            ulong[] roles = GetCommandRoles(command.ToLower(), user.Guild);

            var roleCheck = user.RoleIds.Where(x => roles.Contains(x));

            if (roleCheck.Any())
                return true;
            else
                return false;
        }

        public BsonDocument InsertNewGuild(IGuild guild)
        {
            var guildData = GetGuildData(guild);

            var commandArray = GenerateCommands(guild);

            var doc = new GuildData(guild, commandArray).ToBsonDocument();

            if (guildData == null)
                _GuildInformation.InsertOne(doc);

            return doc;
        }

        public void RemoveGuildData(IGuild guild)
        {
            var guildData = Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString());
            _GuildInformation.DeleteOne(guildData);
        }

        private List<CommandData> GenerateCommands(IGuild guild)
        {
            List<CommandData> arr = new List<CommandData>();

            foreach (var command in DiscordBot.CommandService.Commands)
            {
                arr.Add(new CommandData(command, guild));
            }

            return arr;
        }

        public static AnalyticData GenerateAnalytics(IGuild guild)
        {
            return new AnalyticData(guild)
            {
                InvitesCreated = new List<AnalyticData.Analytic> { new AnalyticData.Analytic()
                {
                    Count = 0,
                    Timestamp = GetCurrentUnixTimestampStatic()
                } },
                InvitesDeleted = new List<AnalyticData.Analytic> { new AnalyticData.Analytic()
                {
                    Count = 0,
                    Timestamp = GetCurrentUnixTimestampStatic()
                } },
                MessagesDeleted = new List<AnalyticData.Analytic> { new AnalyticData.Analytic()
                {
                    Count = 0,
                    Timestamp = GetCurrentUnixTimestampStatic()
                } },
                MessagesReceived = new List<AnalyticData.Analytic> { new AnalyticData.Analytic()
                {
                    Count = 0,
                    Timestamp = GetCurrentUnixTimestampStatic()
                } },
                UsersBanned = new List<AnalyticData.Analytic> { new AnalyticData.Analytic()
                {
                    Count = 0,
                    Timestamp = GetCurrentUnixTimestampStatic()
                } },
                UsersJoined = new List<AnalyticData.Analytic> { new AnalyticData.Analytic()
                {
                    Count = 0,
                    Timestamp = GetCurrentUnixTimestampStatic()
                } },
                UsersLeft = new List<AnalyticData.Analytic> { new AnalyticData.Analytic()
                {
                    Count = 0,
                    Timestamp = GetCurrentUnixTimestampStatic()
                } },
                UsersUnbanned = new List<AnalyticData.Analytic> { new AnalyticData.Analytic()
                {
                    Count = 0,
                    Timestamp = GetCurrentUnixTimestampStatic()
                } }
            };
        }

        public GuildData UpdateAnalytics(AnalyticData data, IGuild guild)
        {
            var guildData = GetGuildData(guild);
            
            guildData.Analytics = data;

            if(DateTime.Now.Subtract(_LastMemberUpdate).TotalMinutes >= 10)
            {
                var approxMemberCountAnalytic = guildData.Analytics.ApproximateMemberCount.Last();
                approxMemberCountAnalytic.Count = (guild as SocketGuild).DownloadedMemberCount;
                guildData.Analytics.ApproximateMemberCount[guildData.Analytics.ApproximateMemberCount.Count - 1] = approxMemberCountAnalytic;
                _LastMemberUpdate = DateTime.Now;
            }

            var set = Builders<BsonDocument>.Update.Set("analytics", guildData.Analytics);
            _GuildInformation.UpdateOne(Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString()), set);
            return guildData;
        }

        public async Task UpdateGuildCommands(IGuild guild)
        {
            var guildData = GetGuildData(guild);

            var oldCommands = guildData.Commands;

            foreach (var command in DiscordBot.CommandService.Commands)
            {
                if (guildData.Commands.FirstOrDefault(x => x.Name == command.Name) == null)
                    guildData.Commands.Add(new CommandData(command, guild));
            }

            guildData.Commands = guildData.Commands.OrderBy(x => x.Name).ToList();
            

            var set = Builders<BsonDocument>.Update.Set("commands", guildData.Commands);
            _GuildInformation.UpdateOne(Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString()), set);
        }

        public void UpdateGuildCommand(IGuild guild, CommandData command)
        {
            var guildData = GetGuildData(guild);
            var cmd = guildData.Commands.FirstOrDefault(x => x.Name == command.Name);

            if (cmd == null) return;

            guildData.Commands.Remove(cmd);
            guildData.Commands.Add(command);

            guildData.Commands = guildData.Commands.OrderBy(x => x.Name).ToList();

            var set = Builders<BsonDocument>.Update.Set("commands", guildData.Commands);

            _GuildInformation.UpdateOne(Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString()), set);
        }

        public async Task UpdateLatency(int latency)
        {
            _LatencyInformation.InsertOne(new AnalyticData.Analytic() { Count = latency }.ToBsonDocument());
        }

        public GuildData GetGuildData(IGuild guild)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString());
            BsonDocument guildData = _GuildInformation.Find(filter).FirstOrDefault();

            if (guildData == null)
                return null;

            var parsedGuildData = BsonSerializer.Deserialize<GuildData>(guildData);
            return parsedGuildData;
        }

        public ulong[] GetCommandRoles(string command, IGuild guild)
        {
            var guildData = GetGuildData(guild);
            var cmd = guildData.Commands.FirstOrDefault(x => x.Name == command.ToLower());

            return cmd.Roles;
        }

        public void ChangeGuildPrefix(IGuild guild, string prefix)
        {
            var set = Builders<BsonDocument>.Update.Set("prefix", prefix);

            _GuildInformation.UpdateOne(Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString()), set);
        }

        public double GetCurrentUnixTimestamp()
        {
            var unixDateTime = (DateTime.Now.ToUniversalTime() - _Epoch).TotalSeconds;
            return unixDateTime;
        }

        public static double GetCurrentUnixTimestampStatic()
        {
            var unixDateTime = (DateTime.Now.ToUniversalTime() - _Epoch).TotalSeconds;
            return unixDateTime;
        }

        public DateTime GetCurrentDateFromUnixTimestamp(double unixTimestamp)
        {
            var timeSpan = TimeSpan.FromSeconds(unixTimestamp);
            var localDateTime = _Epoch.Add(timeSpan).ToLocalTime();
            return localDateTime;
        }

        public async void UpdateGuildInfoLoop()
        {
            while (true)
            {
                var guildInfo = await _GuildInformation.Find(Builders<BsonDocument>.Filter.Empty).ToListAsync();
                var listWrites = new List<WriteModel<BsonDocument>>();
                foreach (var guild in guildInfo)
                {
                    //Convert to readable
                    var guildData = BsonSerializer.Deserialize<GuildData>(guild);
                    //UpdateDefinition<BsonDocument> updateInfo;
                    //int index = guildInfo.IndexOf(guild);

                    //Get unix timestamp analytic and convert
                    var messageDeletedAnalytic = guildData.Analytics.MessagesDeleted.Last();
                    var totalHrs = DateTime.Now.Subtract(GetCurrentDateFromUnixTimestamp(messageDeletedAnalytic.Timestamp)).TotalHours;
                    if (totalHrs >= 24) guildData.Analytics.MessagesDeleted.Add(new AnalyticData.Analytic());

                    var messageReceivedAnalytic = guildData.Analytics.MessagesReceived.Last();
                    totalHrs = DateTime.Now.Subtract(GetCurrentDateFromUnixTimestamp(messageReceivedAnalytic.Timestamp)).TotalHours;
                    if (totalHrs >= 24) guildData.Analytics.MessagesReceived.Add(new AnalyticData.Analytic());

                    var usersJoinedAnalytic = guildData.Analytics.UsersJoined.Last();
                    totalHrs = DateTime.Now.Subtract(GetCurrentDateFromUnixTimestamp(usersJoinedAnalytic.Timestamp)).TotalHours;
                    if (totalHrs >= 24) guildData.Analytics.UsersJoined.Add(new AnalyticData.Analytic());

                    var usersLeftAnalytic = guildData.Analytics.UsersLeft.Last();
                    totalHrs = DateTime.Now.Subtract(GetCurrentDateFromUnixTimestamp(usersLeftAnalytic.Timestamp)).TotalHours;
                    if (totalHrs >= 24) guildData.Analytics.UsersLeft.Add(new AnalyticData.Analytic());

                    var usersBannedAnalytic = guildData.Analytics.UsersBanned.Last();
                    totalHrs = DateTime.Now.Subtract(GetCurrentDateFromUnixTimestamp(usersBannedAnalytic.Timestamp)).TotalHours;
                    if (totalHrs >= 24) guildData.Analytics.UsersBanned.Add(new AnalyticData.Analytic());

                    var usersUnbannedAnalytic = guildData.Analytics.UsersUnbanned.Last();
                    totalHrs = DateTime.Now.Subtract(GetCurrentDateFromUnixTimestamp(usersUnbannedAnalytic.Timestamp)).TotalHours;
                    if (totalHrs >= 24) guildData.Analytics.UsersUnbanned.Add(new AnalyticData.Analytic());

                    var invitesCreatedAnalytic = guildData.Analytics.InvitesCreated.Last();
                    totalHrs = DateTime.Now.Subtract(GetCurrentDateFromUnixTimestamp(invitesCreatedAnalytic.Timestamp)).TotalHours;
                    if (totalHrs >= 24) guildData.Analytics.InvitesCreated.Add(new AnalyticData.Analytic());

                    var invitesDeletedAnalytic = guildData.Analytics.InvitesDeleted.Last();
                    totalHrs = DateTime.Now.Subtract(GetCurrentDateFromUnixTimestamp(invitesDeletedAnalytic.Timestamp)).TotalHours;
                    if (totalHrs >= 24) guildData.Analytics.InvitesDeleted.Add(new AnalyticData.Analytic());

                    var approximateMemberAnalytic = guildData.Analytics.ApproximateMemberCount.Last();
                    totalHrs = DateTime.Now.Subtract(GetCurrentDateFromUnixTimestamp(approximateMemberAnalytic.Timestamp)).TotalHours;
                    if (totalHrs >= 24) guildData.Analytics.ApproximateMemberCount.Add(new AnalyticData.Analytic());

                    //guildInfo[index] = guildData.ToBsonDocument();
                    var set = Builders<BsonDocument>.Update.Set("analytics", guildData.Analytics);
                    var filter = Builders<BsonDocument>.Filter.Eq("guild_id", guildData.guildID.ToString());
                    //await _GuildInformation.UpdateOneAsync(filter, set);
                    listWrites.Add(new UpdateOneModel<BsonDocument>(filter, set));
                }

                await _GuildInformation.BulkWriteAsync(listWrites);

                Console.WriteLine($"Updated database info.");
                Thread.Sleep(60000);
            }
        }
    }
}
