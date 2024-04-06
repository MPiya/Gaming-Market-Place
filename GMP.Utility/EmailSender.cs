using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.Extensions.Configuration;

namespace F2Play.Utility
{
    public class EmailSender : IEmailSender
    {

        private readonly string _emailSender;
        private readonly string _password;

        public EmailSender(IConfiguration configuration)
        {
            _emailSender = configuration["EmailSender:Email"];
            _password = configuration["EmailSender:Password"];
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse(_emailSender));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };


            //send email
            using (var emailClient = new SmtpClient())
            {

               

                emailClient.Connect("smtp-mail.outlook.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate(_emailSender, _password);
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);

                return Task.CompletedTask;
            }

            //    var mail = "bonissocool@hotmail.com";
            //    var pw = "Mpiyacs321";

            //    var client = new System.Net.Mail.SmtpClient("smtp.office365.com", 587)
            //    {

            //        EnableSsl = true,
            //        UseDefaultCredentials = false,
            //        Credentials = new NetworkCredential(mail, pw)
            //    };

            //    return client.SendMailAsync(
            //        new MailMessage(from: mail,
            //                        to: email,
            //                        subject,
            //                        htmlMessage
            //                        ));
            //}


        }
    }
}