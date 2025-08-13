using MailKit.Security;
using MimeKit;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.Entities;
using System;
using System.Net;
using System.Net.Mail;
using MeowWoofSocial.Data.DTO.Custom;

namespace MeowWoofSocial.Business.Ultilities.Email
{
    public class Email : IEmail
    {
        public Email()
        {
        }

        public async Task SendEmail(string Subject, List<EmailReqModel> emailReqModels)
        {
            try
            {
                string from = Environment.GetEnvironmentVariable("EMAIL_LOGIN") ?? throw new CustomException("Email login not configured.");
                string pass = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? throw new CustomException("Email password not configured.");

                using MailKit.Net.Smtp.SmtpClient smtp = new();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(from, pass);

                foreach (var item in emailReqModels)
                {
                    MimeMessage message = new();
                    message.From.Add(MailboxAddress.Parse("admin@buubuu.id.vn"));
                    message.To.Add(MailboxAddress.Parse(item.Email));
                    message.Subject = Subject;

                    var bodyBuilder = new BodyBuilder
                    {
                        HtmlBody = item.HtmlContent
                    };

                    var logoPath = "./MeowWoofLogo.webp";
                    var logoImage = bodyBuilder.LinkedResources.Add(logoPath);
                    logoImage.ContentId = "MeowWoofLogo";

                    // Gán body vào message
                    message.Body = bodyBuilder.ToMessageBody();

                    // Gửi email
                    await smtp.SendAsync(message);
                }

                await smtp.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error sending email: " + e.Message);
            }
        }


        //public async Task<bool> SendListEmail(string Subject, List<EmailSendingModel> sendingList)
        //{
        //    try
        //    {
        //        //string from = "esmsweb@gmail.com";
        //        //string pass = "pzsz yhqj zcem dacg";
        //        string from = "fresheracademy.ms@gmail.com";
        //        string pass = "wgov rkcz gyfm uleu";
        //        using MailKit.Net.Smtp.SmtpClient smtp = new();
        //        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        //        await smtp.AuthenticateAsync(from, pass);
        //        var options = new PusherOptions
        //        {
        //            Cluster = "ap1",
        //            Encrypted = true,
        //        };

        //        var pusher = new Pusher("1766995", "a561cc1f29b052bf7808", "9e7e45a4fcf5ad651d3b", options);
        //        decimal count = 0;
        //        foreach (EmailSendingModel model in sendingList)
        //        {
        //            var toEmail = model.email;
        //            MimeMessage message = new();
        //            message.From.Add(MailboxAddress.Parse(from));
        //            message.Subject = "[FAMS] " + Subject;
        //            message.To.Add(MailboxAddress.Parse(toEmail));
        //            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        //            {
        //                Text = model.html
        //            };
        //            _ = await smtp.SendAsync(message);
        //            count++;
        //            decimal percent = count / sendingList.Count * 100;
        //            if (count % 2 == 0)
        //            {
        //                await pusher.TriggerAsync("my-channel", "my-event", new { count, total = sendingList.Count, percent });
        //            }
        //        }
        //        await pusher.TriggerAsync("my-channel", "my-event", new { count, total = sendingList.Count, percent = 100 });
        //        await smtp.DisconnectAsync(true);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}
    }
}
