using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Utils;


namespace ContentFactory.Services
{
    public class EmailService : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("ContentFactory", "no-replay@contentfactory.store"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("mail.hosting.reg.ru", 465, true);//25///465 ssl
                await client.AuthenticateAsync("no-replay@contentfactory.store", "31D#oq0x");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

        public async Task SendMailsAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("ContentFactory", "no - replay@contentfactory.store"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            string current = Environment.CurrentDirectory;

            string FullFormatPath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "images", "logo.png");
            string HtmlFormat = string.Empty;
            HtmlFormat = message;
            var bodyBuilder = new BodyBuilder();
            var image = bodyBuilder.LinkedResources.Add(FullFormatPath);
            image.ContentId = MimeUtils.GenerateMessageId();
            //HtmlFormat = HtmlFormat.Replace(Path.GetFileName(FullFormatPath), string.Format("cid:{0}", image.ContentId));
            HtmlFormat = HtmlFormat.Replace("{0}", image.ContentId);
            // bodyBuilder.HtmlBody = string.Format(message, image.ContentId);
            bodyBuilder.HtmlBody = HtmlFormat;

            //  bodyBuilder.HtmlBody =  message;
            emailMessage.Body = bodyBuilder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("mail.hosting.reg.ru", 465, true);//25///465 ssl
                await client.AuthenticateAsync("no-replay@contentfactory.store", "31D#oq0x");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
