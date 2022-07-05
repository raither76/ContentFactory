using Telegram.Bot;
using Telegram.Bot.Types;
namespace ContentFactory.Code;


public class StartCommand : Command
{
    public override string Name => @"/start";

    public override bool Contains(Message message)
    {
        if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }

    public override async Task Execute(Message message, TelegramBotClient botClient)
    {
        var chatId = message.Chat.Id;
        await botClient.SendTextMessageAsync(chatId, "Hallo I'm ASP.NET Core Bot", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
    }
}
