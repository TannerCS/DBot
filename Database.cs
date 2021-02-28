using DBot.Constants;
using Discord;
using Discord.Commands;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public BsonDocument InsertNewGuild(IGuild guild)
        {
            var guildData = GetGuildData(guild);

            var commandArray = GenerateCommands(guild);

            var doc = new BsonDocument {
                { "guild_id", guild.Id.ToString() },
                { "commands", commandArray},
                { "prefix", "!" }
            };

            if (guildData == null)
                _GuildInformation.InsertOne(doc);

            return doc;
        }

        private BsonArray GenerateCommands(IGuild guild)
        {
            BsonArray arr = new BsonArray();

            foreach (var command in DiscordBot.CommandService.Commands)
            {
                arr.Add(new CommandData(command, guild).ToBsonDocument());
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

        public void ChangeGuildPrefix(IGuild guild, string prefix)
        {
            var set = Builders<BsonDocument>.Update.Set("prefix", prefix);

            _GuildInformation.UpdateOne(Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString()), set);
        }
    }
}
