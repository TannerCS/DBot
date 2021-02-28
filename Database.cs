using DBot.Constants;
using Discord;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DBot
{
    public class Database
    {
        private MongoClient _DBClient;
        private IMongoDatabase _Database;
        private IMongoCollection<BsonDocument> _GuildInformation;

        public Database()
        {
            _DBClient = new MongoClient(DiscordBot.Config.Data.mongoUrl);

            _Database = _DBClient.GetDatabase("discord-bot");
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

        private List<CommandData> GenerateCommands(IGuild guild)
        {
            List<CommandData> arr = new List<CommandData>();

            foreach (var command in DiscordBot.CommandService.Commands)
            {
                arr.Add(new CommandData(command, guild));
            }

            return arr;
        }

        public void UpdateGuildCommands(IGuild guild)
        {
            var guildData = GetGuildData(guild);

            var oldCommands = guildData.Commands;

            foreach (var command in DiscordBot.CommandService.Commands)
            {
                if (guildData.Commands.FirstOrDefault(x => x.Name == command.Name) == null)
                    guildData.Commands.Add(new CommandData(command, guild));
            }

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

            var set = Builders<BsonDocument>.Update.Set("commands", guildData.Commands);

            _GuildInformation.UpdateOne(Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString()), set);
        }

        public GuildData GetGuildData(IGuild guild)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString());
            var guildData = _GuildInformation.Find(filter).FirstOrDefault();

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
    }
}
