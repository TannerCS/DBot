using Discord;
using Discord.Commands;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;

namespace DBot.Constants
{
    public class CommandData
    {
        public CommandData(CommandInfo command, IGuild guild)
        {
            Name = command.Name;
            Description = command.Summary;
            Aliases = command.Aliases.ToArray();
            Roles = new ulong[] { guild.EveryoneRole.Id };
            Enabled = true;
        }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Aliases")]
        public string[] Aliases { get; set; }

        [BsonElement("Roles")]
        public ulong[] Roles { get; set; }

        [BsonElement("Enabled")]
        public bool Enabled { get; set; }
    }
}
