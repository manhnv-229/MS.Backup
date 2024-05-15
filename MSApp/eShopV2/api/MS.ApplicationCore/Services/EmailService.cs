using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Services
{
    internal class EmailService
    {
        String SendMailFrom = "daotao.misa.edu@gmail.com";
        String SendMailTo = "manhnv229@gmail.com";
        String SendMailSubject = "Xác thực tài khoản daotao.misa";
        String SendMailBody = "Mã xác minh của bạn là 1111";
        public EmailService(string sentTo, string titleEmail, string contentEmail)
        {
            this.SendMailTo = sentTo;
            SendMailSubject = titleEmail;
            SendMailBody = contentEmail;
        }
        public bool SentEmail()
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage email = new MailMessage();
                // START
                email.From = new MailAddress(SendMailFrom);
                email.To.Add(SendMailTo);
                email.CC.Add(SendMailFrom);
                email.Subject = SendMailSubject;
                email.Body = SendMailBody;
                //END
                SmtpServer.Timeout = 5000;
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential(SendMailFrom, "ospbnykljjqzyynu");
                SmtpServer.Send(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
