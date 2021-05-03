using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DBot.Services
{
    public class LoggingService
    {
        public LoggingService(DiscordSocketClient client, CommandService command)
        {
            client.Log += LogAsync;
            command.Log += LogAsync;
        }
        public async Task LogAsync(LogMessage message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now}]");

            switch (message.Severity)
            {
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("[INFO] ");
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("[DEBUG] ");
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("[VERBOSE] ");
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("[WARNING] ");
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[ERROR] ");
                    break;
                case LogSeverity.Critical:
                    await File.AppendAllTextAsync($"/logs/{DateTime.Now.ToString().Split(' ')[0]}.txt", $"[{DateTime.Now}] [CRITICAL] {message.Source}\n{message.Message}\n{message.Exception}\n\n");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("[CRITICAL] ");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            
            if (message.Exception is CommandException cmdException)
            {
                await File.AppendAllTextAsync($"/logs/{DateTime.Now.ToString().Split(' ')[0]}.txt", 
                    $"[{DateTime.Now}] [CRITICAL] [{cmdException.Command.Name}]\n" +
                    $"parameters: {string.Join(" ", cmdException.Command.Parameters)}\n" +
                    $"Guild: {cmdException.Context.Guild.Name} ({cmdException.Context.Guild.Id})\n" +
                    $"Channel: {cmdException.Context.Channel.Name} ({cmdException.Context.Channel.Id})\n" + 
                    $"Author: {cmdException.Context.Message.Author.Username}${cmdException.Context.Message.Author.Discriminator} ({cmdException.Context.Message.Author.Id})\n" + 
                    $"Source: {cmdException.Source}\n" + 
                    $"Stacktrace: {cmdException.StackTrace}\n");
                Console.Write($"{cmdException.Command.Aliases[0]} failed to execute in {cmdException.Context.Channel.Name}");
            }
            else
            {
                Console.WriteLine(message.Message);
            }

            Console.ResetColor();
        }

        public void Log(LogMessage message)
        {
            LogAsync(message);
        }
    }
}