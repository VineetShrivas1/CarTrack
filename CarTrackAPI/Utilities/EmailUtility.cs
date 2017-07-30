using CarTrack.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarTrackAPI.Utilities
{
    public class EmaiUtility
    {
        // Note: To send email you need to add actual email id and credential so that it will work as expected  
        public static readonly string EMAIL_SENDER = "vineetshrivas@hotmail.com"; // change it to actual sender email id or get it from UI input  
        public static readonly string EMAIL_CREDENTIALS = "Sunil13121979#33"; // Provide credentials   
        public static readonly string SMTP_CLIENT = "smtp-mail.outlook.com"; // as we are using outlook so we have provided smtp-mail.outlook.com   
        public static readonly string EMAIL_BODY = "Reset your Password <a href='http://{0}.safetychain.com/api/Account/forgotPassword?{1}'>Here.</a>";
        private static string senderAddress;
        private static string clientAddress;
        private static string netPassword;
        public EmaiUtility(string sender, string Password, string client)
        {
            senderAddress = sender;
            netPassword = Password;
            clientAddress = client;
        }
        public static bool SendEmail(string recipient, string subject, string message)
        {
            bool isMessageSent = false;
            //Intialise Parameters  
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            //client.Port = 587;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(senderAddress, netPassword);
            //client.EnableSsl = true;
            //client.Credentials = credentials;
            try
            {
                //var to = new System.Net.Mail.MailAddressCollection();
                //to.Add(recipient.Trim());
                var mail = new System.Net.Mail.MailMessage();
                mail.To.Add(recipient.Trim());
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                //System.Net.Mail.Attachment attachment;  
                //attachment = new Attachment(@"C:\Users\XXX\XXX\XXX.jpg");  
                //mail.Attachments.Add(attachment);  
                client.Send(mail);
                isMessageSent = true;
            }
            catch (Exception ex)
            {
                isMessageSent = false;

                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(new Exception(ex.Message+"\r\n:"+ex.StackTrace)));
            }
            return isMessageSent;
        }

        public static EmailTemplate GetEmailTemplate(string type)
        {
            using (CarTrackEntities myContext = new CarTrackEntities())
            {
                return myContext.EmailTemplates.FirstOrDefault(x => x.TemplateType == type);
            }
        }
    }
}