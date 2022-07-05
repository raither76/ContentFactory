using ContentFactory.Services;
using Microsoft.AspNetCore.Mvc;
using TL;

namespace ContentFactory.Controllers;

[ApiController]
[Route("[controller]")]
public class CFBotController : ControllerBase
{
    private readonly CFTelegramService CFT;
    public CFBotController(CFTelegramService cft) => CFT = cft;

    [HttpGet("status")]
    public async Task<ContentResult> Status()
    {
        var config = await CFT.ConfigNeeded();
        if (config != null)
            return Content($@"Enter {config}: <form action=""config""><input name=""value"" autofocus/></form>", "text/html");
        else
            return Content($@"Connected as {CFT.User}<br/><a href=""chats"">Get all chats</a>", "text/html");
    }

    [HttpGet("config")]
    public ActionResult Config(string value)
    {
        CFT.ReplyConfig(value);
        return Redirect("status");
    }

    [HttpGet("chats")]
    public async Task<object> Chats()
    {
        if (CFT.User == null) throw new Exception("Complete the login first");
        var chats = await CFT.Client.Messages_GetAllChats(null);
        return chats.chats;
    }
    [HttpGet("send")]
    public async Task<object> Send()
    {
        if (CFT.User == null) throw new Exception("Complete the login first");

        CFT.SendMessage();
        return null;
    }
}
