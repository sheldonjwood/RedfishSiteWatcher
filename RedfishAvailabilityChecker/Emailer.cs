using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedfishAvailabilityChecker
{
    internal class Emailer
    {
        public void SendEmail(string body)
        {
            var initializer = new BaseClientService.Initializer();
            var service = new GmailService();

            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress("sheldonjwood@gmail.com");
            mailMessage.To.Add("sheldonw@conservice.com");
            mailMessage.ReplyToList.Add("sheldonjwood@gmail.com");
            mailMessage.Subject = "Redfish Sites Available";
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = false;

            var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);

            var gmailMessage = new Google.Apis.Gmail.v1.Data.Message
            {
                Raw = Encode(mimeMessage)
            };

            Google.Apis.Gmail.v1.UsersResource.MessagesResource.SendRequest request = service.Users.Messages.Send(gmailMessage, "");

            request.Execute();
        }

        public static string Encode(MimeMessage mimeMessage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                mimeMessage.WriteTo(ms);
                return Convert.ToBase64String(ms.GetBuffer())
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }
    }
}
