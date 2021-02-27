using System;
using System.Threading.Tasks;

namespace DBot
{
    class Program
    {
        public static DiscordBot DiscordBot;
        static void Main(string[] args)
        {
            DiscordBot = new DiscordBot();
            DiscordBot.MainAsync().GetAwaiter().GetResult();
        }
    }
}
