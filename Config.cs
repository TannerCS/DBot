using DBot.Constants;
using Newtonsoft.Json;

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
