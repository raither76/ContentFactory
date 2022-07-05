
namespace ContentFactory.Services
{
    public interface ICFTelegramService
    {
        TL.User User { get; }
        void AddContactAsync(Models.User user);
        Task<string> ConfigNeeded();
        void ReplyConfig(string value);
        Task<Models.User> SendConfirmAsync(Models.User user, string msg);
        Task<bool> SendDocAsync(List<string> files);
        void SendMessage();
    }
}