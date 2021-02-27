using DBot.Constants;
using DBot.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBot
{
    public class Config
    {
        public ConfigData Data;
        public Config()
        {
            string json = System.IO.File.ReadAllText("./config.json");
            Data = JsonConvert.DeserializeObject<ConfigData>(json);
            DiscordBot.Logger.LogAsync(new Discord.LogMessage(Discord.LogSeverity.Info, "Config", "Config successfully loaded."));
        }
    }
}
