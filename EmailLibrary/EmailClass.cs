using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace EmailLibrary
{
    public class EmailClass
    {
        public static MailMessage mail = null;

        public static string SendMail(Email email)
        {
            try
            {
                //string sender = "a_lgz@163.com";
                //string toReciver = "26264060@qq.com"; "smtp.163.com" 25
                //实例化一个发送邮件类
                mail = new MailMessage(email.FromEmail, email.ToEmail);

                SmtpClient host = new SmtpClient(email.Host, email.Port);
                host.Credentials = new System.Net.NetworkCredential(email.Username, email.Password);

                ////收件人邮箱地址
                //mail.To.Add(email.ToEmail);
                mail.Subject = email.Subject;

                mail.Body = email.Body;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = System.Text.Encoding.UTF8;

                host.Send(mail);
            }catch(Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public string ReciverEmail()
        {
            return "";
        }
    }
}
