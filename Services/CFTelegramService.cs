using ContentFactory.Code;
using TL;
namespace ContentFactory.Services;


public class CFTelegramService : BackgroundService, ICFTelegramService
{
    public readonly WTelegram.Client Client;
    public TL.User User { get; private set; }
    public Task<string> ConfigNeeded() => _configNeeded.Task;

    private readonly IConfiguration _config;
    private TaskCompletionSource<string> _configNeeded = new();
    private readonly ManualResetEventSlim _configRequest = new();
    private string _configValue;
    private IWebHostEnvironment _appEnvironment;

    public CFTelegramService(IConfiguration config, ILogger<CFTelegramService> logger, IWebHostEnvironment appEnvironment)
    {
        _config = config;
        _appEnvironment = appEnvironment;
        WTelegram.Helpers.Log = (lvl, msg) => logger.Log((LogLevel)lvl, msg);
        Client = new WTelegram.Client(Config);
    }


    private string Config(string what)
    {
        var path = _appEnvironment.ContentRootPath + "\\Wtelegram\\";
        switch (what)
        {
            case "session_pathname": return Path.Combine(path, "WTelegram.session");
            case "api_id": return "13949166";
            case "api_hash": return "ecc165de859e8f2b4c6472dc6cfc606f";
            case "phone_number": return "+79640547152";
            case "verification_code": return _config["Manager:code"];
            case "first_name": return "CFAPI";      // if sign-up is required
            case "last_name": return "----";        // if sign-up is required
            case "password": return "----";     // if user has enabled 2FA
            default: return null;                  // let WTelegramClient decide the default config
        }
    }
    public void ReplyConfig(string value)
    {
        _configValue = value;
        _configNeeded = new();
        _configRequest.Set();
    }
    public async void SendMessage()
    {
        string manager = _config["Manager:Telega"];
        var resolved = await Client.Contacts_ResolveUsername(manager); // username without the @
        await Client.SendMessageAsync(resolved, "Проверка связи");
    }
    public async Task<Models.User> SendConfirmAsync(Models.User user, string msg)
    {
        var resolved = await Client.Contacts_ResolveUsername(user.TelegramId.Replace("@", "")); // username without the @
        var result = await Client.SendMessageAsync(resolved, msg);
        if (result.peer_id != null)
        {
            string phone = user.Phone.Replace("(", "").Replace(")", "").Replace(" ", "");
            var contacts = await Client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = phone } });
            //if (contacts.imported.Length > 0)
            //	await Client.SendMessageAsync(contacts.users[contacts.imported[0].user_id], "Hello!");
            //await Client.Contacts_AddContact(new InputPhoneContact { phone = phone }, user.Nick, "", phone);

            user.IsTelegramConfirmed = true;
            user.HashLink = Coder.Encrypt($"{user.Email}-{user.SecurityStamp}", user.Nick);
            var text = $"Добрый день, вы зарегистрировались в фотостудии ContetntFactory\n" +
                $"Ваш Nick: <b>{HtmlText.Escape(user.Nick)}</b>\n" +
                $"Ваша персональная ссылка для входа в студию: <a href=\"https://contentfactory.store/Home/Login?nick={user.HashLink}\"><b>Войти в студию</b></a>\n" +
                $"<div style=\"color:red;\"><b>Не передавайте ее никому</b></div>";
            var entities = Client.HtmlToEntities(ref text);

            await Client.SendMessageAsync(resolved, text, entities: entities);

        }
        return user;
    }
    public async void AddContactAsync(Models.User user)
    {

    }

    public async Task<bool> SendDocAsync(List<string> files)
    {
        bool res = false;
        string manager = _config["Manager:Telega"];
        var resolved = await Client.Contacts_ResolveUsername(manager);
        for (int i = 0; i < files.Count(); i++)
        {
            var uploadedFile = await Client.UploadFileAsync(files[i]);
            var result = await Client.SendMediaAsync(resolved, "-=ContentFactory.store=- (Документы к заказу)", uploadedFile);
            if (result.peer_id != null) res = true;
        };
        return res;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            User = await Client.LoginUserIfNeeded();
        }
        catch (Exception ex)
        {
            _configNeeded.SetException(ex);
            throw;
        }
        _configNeeded.SetResult(null); // login complete
    }
}