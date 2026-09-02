using notification_service.Notifications.DTOS;
using notification_service.Notifications.Services;
using System.Net.Mail;
using System.Net;

namespace notification_service.Notification.Application.Services
{
    public class Emails : INotifications
    {
        private IConfiguration _config;

        public Emails(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string TypeService => "Email";


        
        public async Task<bool> SendNotification(RequestSendMessage request)
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(_config["Email"], _config["EmailKey"]);
                    smtp.EnableSsl = true;

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(_config["Email"]!, _config["Name"]);
                        message.To.Add(request.To);
                        message.Subject = request.Subject;
                        message.Body = request.Body;
                        message.IsBodyHtml = true;

                        await smtp.SendMailAsync(message);
                    }
                }

                Console.WriteLine("Email sent successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                return false;
            }
        }
    }
}
