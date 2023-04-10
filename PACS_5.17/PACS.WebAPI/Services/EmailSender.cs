using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PACS.WebAPI.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = new MailMessage(
               new MailAddress("1023998223@qq.com", "IdentityDemo"),
               new MailAddress(email)
               );
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;
            mail.Priority = MailPriority.High;//邮件优先级
                                              // 设置SMTP服务器
            var smtp = new SmtpClient("smtp.qq.com", 587);
            smtp.EnableSsl = true;
            //smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("1023998223@qq.com", "tzjreomxsbxobeie");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            await smtp.SendMailAsync(mail);
        }
    }
}
