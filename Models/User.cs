using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;


namespace ContentFactory.Models;

public class User : IdentityUser
{

    [Required]
    [Display(Name = "Имя")]
    public string? Nick { get; set; } = "";
    [Display(Name = "Бренд")]
    public string? Brend { get; set; } = "";
    [Phone]
    [Display(Name = "Телефон")]
    public string? Phone { get; set; }
    [Display(Name = "Telegram ID")]
    public string? TelegramId { get; set; }
    [Display(Name = "Название организации")]
    public string? CompanyName { get; set; }
    [Display(Name = "ИНН")]
    public string? INN { get; set; }
    [Display(Name = "ИНН")]
    public string? INN1 { get; set; }
    [Display(Name = "КПП")]
    public string? KPP { get; set; }
    [Display(Name = "Генеральный директор")]
    public string? HeadOfCompany { get; set; }
    [Display(Name = "ФИО")]
    public string? FullName { get; set; }
    [Required]
    [Display(Name = "Тип пользователя")]
    public bool IsTelegramConfirmed { get; set; }
    public DateTime RegDate { get; set; }
    public string? RegIp { get; set; }
    public string? HashLink { get; set; }
    public int PaymentType { get; set; }
    public long? PeerId { get; set; }

    public async Task<string> GetVcard(string path)
    {
        VCard _vCard = new VCard() { FirstName = Nick, Mobile = Phone, Organization=Brend };
        var vCardfileName = _vCard.GetFullName() + ".vcf";
        var cardpath = Path.Combine(path, vCardfileName);
        using (StreamWriter writer = new StreamWriter(cardpath, false))
        {
            await writer.WriteLineAsync(_vCard.ToString());
            writer.Close();
        }

        return cardpath;

    }


    public async Task CreateUserAsync(IHttpContextAccessor accessor,
        UserManager<Models.User> userManager,
        IUserStore<Models.User> userStore,
        SignInManager<Models.User> signInManager)
    {
        try
        {
            var result = string.Empty;

            if (accessor.HttpContext.Request.Headers != null)
            {
                var forwardedHeader = accessor.HttpContext.Request.Headers["X-Forwarded-For"];
                if (!StringValues.IsNullOrEmpty(forwardedHeader))
                    result = forwardedHeader.FirstOrDefault();
            }
            if (string.IsNullOrEmpty(result) && accessor.HttpContext.Connection.RemoteIpAddress != null)
                result = accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (result == null || result == "" || result.Length < 6)
            {
                result = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(1).ToString();

            }
            else if (result == null || result == "" || result.Length < 6)
            {
                result = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(1).ToString();
            }
            else { result = "Новый пользователь"; }
            var user = Activator.CreateInstance<Models.User>();
            user.RegIp = result;
            user.EmailConfirmed = true;
            user.RegDate = DateTime.Now;
            if (user != null)
            {
                await userManager.AddToRoleAsync(user, "Пользователь");
            }
            user.Email = $"{Id.ToString().Replace("-", string.Empty)}@ContentFactory.store";
            user.UserName = user.Email;
            user.NormalizedEmail = user.Email.ToUpper();
            await userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
            await userStore.SetNormalizedUserNameAsync(user, user.UserName.ToUpper(), CancellationToken.None);
            // await userStore.UpdateAsync(user, CancellationToken.None);
            var res = await userManager.CreateAsync(user, "_qWeRty123_");

            if (res.Succeeded)
            {

                await signInManager.PasswordSignInAsync(user.Email, "_qWeRty123_", true, lockoutOnFailure: false);
                //await signInManager.SignInAsync(user, isPersistent: false);
            }


        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(Models.User)}'. " +
                $"Ensure that '{nameof(Models.User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }


    }

}

