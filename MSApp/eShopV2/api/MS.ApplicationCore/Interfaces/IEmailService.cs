using MS.ApplicationCore.Entities.Auth;
using MS.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.ApplicationCore.Interfaces
{
    public interface IEmailService
    {
        //string SendMailFrom { get; set; }
        string SendMailTo { get; set; }
        //string SendMailSubject { get; set; }
        //string SendMailBody { get; set; }

        public UserTokenConfirm SentMailConfirmToken(User user);
        public bool SentEmail(string sentTo, string titleEmail = "", string contentEmail = "");

    }
}
