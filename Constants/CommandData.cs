using Discord;
using Discord.Commands;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
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
            var directory = $"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Commands/";
            List<string> folders = new DirectoryInfo(directory)
                .EnumerateFiles($"{command.Module.Name}.cs", SearchOption.AllDirectories)
                .Select(d => d.Directory.Name).ToList();
            Category = folders[0];
        }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Aliases")]
        public string[] Aliases { get; set; }

        [BsonElement("Roles")]
        public ulong[] Roles { get; set; }

        [BsonElement("Enabled")]
        public bool Enabled { get; set; }
    }
}
