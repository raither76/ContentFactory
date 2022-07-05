using ContentFactory.Models;
using System.ComponentModel.DataAnnotations;

namespace ContentFactory.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
        }
        public RegisterViewModel(User user, int orderId)
        {
            Nick = user.Nick;
            Brend = user.Brend;
            Phone = user.Phone;
            TelegramId = user.TelegramId;
            CompanyName = user.CompanyName;
            INN = user.INN;
            INN1 = user.INN1;
            KPP = user.KPP;
            HeadOfCompany = user.HeadOfCompany;
            FullName = user.FullName;
            PaymentType = user.PaymentType;
            OrderId = orderId;

        }
        public async Task<User> PutUser(User user)
        {

            user.Nick = Nick;
            user.Brend = Brend;
            user.Phone = Phone;
            user.TelegramId = TelegramId;
            user.CompanyName = CompanyName;
            user.INN = INN;
            user.INN1 = INN1;
            user.KPP = KPP;
            user.HeadOfCompany = HeadOfCompany;
            user.FullName = FullName;
            user.PaymentType = PaymentType;

            return user;

        }
        [Required]
        [Display(Name = "Имя")]
        public string? Nick { get; set; } = "";
        [Display(Name = "Бренд")]
        public string? Brend { get; set; }
        [Phone]
        [Required]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
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
        public int PaymentType { get; set; }
        public int OrderId { get; set; }
        public string ErrMessage { get; set; }



    }
}
