using MS.Core.Entities.Auth;
using MS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MS.Core.Interfaces;
using MS.Core.Utilities;

namespace MS.Infrastructure
{
    public class EmailService : IEmailService
    {
        string _sendMailFrom = "nmanh.com@gmail.com";
        string _sendMailTo = "manhnv229@gmail.com";
        string _sendMailSubject = "Xác thực tài khoản nmanh.com";
        string _sendMailBody = "Mã xác minh của bạn là 1111";
        //public EmailService(string sentTo, string titleEmail = "", string contentEmail = "")
        //{
        //    this._sendMailTo = sentTo;
        //    _sendMailSubject = titleEmail;
        //    _sendMailBody = contentEmail;
        //}

        public string SendMailTo { get => _sendMailTo; set => _sendMailTo = value; }
        //public string SendMailFrom { get => _sendMailFrom; set => _sendMailFrom = value; }
        //public string SendMailSubject { get => _sendMailSubject; set => _sendMailSubject = value; }
        //public string SendMailBody { get => _sendMailBody; set => _sendMailBody = value; }

        public bool SentEmail(string sentTo, string titleEmail = "", string contentEmail = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(sentTo))
                    _sendMailTo = sentTo;

                if (!string.IsNullOrEmpty(titleEmail))
                    _sendMailSubject = titleEmail;

                if (!string.IsNullOrEmpty(contentEmail))
                    _sendMailBody = contentEmail;

                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage email = new MailMessage();

                // START
                email.From = new MailAddress(_sendMailFrom);
                email.To.Add(_sendMailTo);
                email.CC.Add(_sendMailFrom);
                email.Subject = _sendMailSubject;
                email.Body = _sendMailBody;
                email.IsBodyHtml = true;

                //END
                SmtpServer.Timeout = 5000;
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential(_sendMailFrom, "gbxqacpqftbytfso");
                SmtpServer.Send(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Tạo Token, gửi Email và trả về thông tin Token
        /// </summary>
        /// <param name="user">User cần xử lý</param>
        /// <returns>Thông tin Token</returns>
        /// CreatedBy: NVMANH (18/08/2022)
        public UserTokenConfirm SentMailConfirmToken(User user)
        {
            // Tạo mã xác nhận:
            var tokenCode = GenTokenCodeConfirm();
            var emailContent = $"Chúc mừng <b>{user.Organization.OrganizationName}</b>! <br/>" +
                $"Mã xác minh truy để xác nhận tài khoản của bạn là <b>{tokenCode}</b>. <br/>" +
                "Vui lòng nhập mã xác nhận trong ứng dụng để kích hoạt tài khoản của bạn.<br/>" +
                "<b>Mạnh Software</b> chân thành cám ơn bạn!";
            //var emailService = new EmailService(user.Employee.Email, "[MISA JSC] Xác thực tài khoản hệ thống đào tạo MISA FRESHER!", emailContent);
            var isSend = SentEmail(user.Email, $"[{tokenCode}] Mã xác thực cửa hàng [{user.Organization.OrganizationName}]", emailContent);
            if (isSend)
            {
                var expireDate = CommonFunction.ConvertDateToVNTime(DateTime.Now.AddDays(1));
                return new UserTokenConfirm()
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    TokenCode = tokenCode,
                    ExpireDate = expireDate,
                    OrganizationId = user.OrganizationId,
                    Email = user.Email
                };
            }
            else
                return null;
        }

        /// <summary>
        /// Gen mã Token ngẫu nhiên
        /// </summary>
        /// <returns>Chuỗi 6 chữ số được sinh ngẫu nhiên</returns>
        /// CreatedBy: NVMANH (18/08/2022)
        private string GenTokenCodeConfirm()
        {
            StringBuilder sb = new StringBuilder();
            //char c;
            string c;
            Random rand = new Random();
            for (int i = 0; i < 6; i++)
            {
                //c = Convert.ToChar(Convert.ToInt32(rand.Next(65, 87)));
                c = Convert.ToInt32(rand.Next(1, 9)).ToString();
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
