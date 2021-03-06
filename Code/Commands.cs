using Telegram.Bot;
using Telegram.Bot.Types;
namespace ContentFactory.Code;

public abstract class Command
{
    public abstract string Name { get; }

    public abstract Task Execute(Message message, TelegramBotClient client);

    public abstract bool Contains(Message message);
}