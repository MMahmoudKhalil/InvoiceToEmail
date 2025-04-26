using System.Net.Mail;
using System.Net;

namespace InvoiceToEmail.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "الايميل الي بيرسل الفواتير";
        private readonly string _smtpPass = "الباسورد";

        public void SendEmailWithAttachment(string toEmail, string subject, string body, byte[] attachmentBytes, string attachmentName)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_smtpUser);
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = body;

                if (attachmentBytes != null)
                {
                    var attachment = new Attachment(new MemoryStream(attachmentBytes), attachmentName);
                    mailMessage.Attachments.Add(attachment);
                }

                client.Send(mailMessage);
            }
        }
    }
}