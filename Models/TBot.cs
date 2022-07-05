using ContentFactory.Code;
using Telegram.Bot;

namespace ContentFactory.Models;
public class TBot
{
    private static TelegramBotClient botClient;
    private static List<Command> commandsList;

    public static IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

    public static async Task<TelegramBotClient> GetBotClientAsync()
    {
        if (botClient != null)
        {
            return botClient;
        }


        commandsList = new List<Command>();
        commandsList.Add(new StartCommand());
        //TODO: Add more commands

        botClient = new TelegramBotClient(BotConfiguration.BotToken);
        string hook = string.Format(BotConfiguration.HostAddress, "/bot");
        await botClient.SetWebhookAsync(hook);
        return botClient;
    }
}
