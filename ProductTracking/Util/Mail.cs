using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text;

namespace ProductTracking.Util
{
    public class Mail
    {
        public static void ReportMail(String subject, string mailBody)
        {
            List<string> ReportToMailIds = new List<string>();
            List<string> ReportCcMailIds = new List<string>();
            string toIdStrings = System.Configuration.ConfigurationManager.AppSettings["report_to"];
            string ccIdStrings = System.Configuration.ConfigurationManager.AppSettings["report_cc"];
            foreach (string toMailId in toIdStrings.Split(';'))
                ReportToMailIds.Add(toMailId);
            foreach (string ccMailId in ccIdStrings.Split(';'))
                ReportCcMailIds.Add(ccMailId);
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = System.Configuration.ConfigurationManager.AppSettings["Email_Host"];
            client.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["EmailID_USER"], System.Configuration.ConfigurationManager.AppSettings["Email_PASSWORD"]);
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["SenderEmail"], "New Shipment Details");
            if (ReportToMailIds.Count > 0)
            {
                foreach (string toAddress in ReportToMailIds)
                    if (!String.IsNullOrEmpty(toAddress))
                        mailMessage.To.Add(toAddress);
                foreach (string ccAddress in ReportCcMailIds)
                    if (!String.IsNullOrEmpty(ccAddress))
                        if (!String.IsNullOrEmpty(ccAddress))
                            mailMessage.CC.Add(ccAddress);
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = subject;
                mailMessage.Body = mailBody;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(mailMessage);
            }
        }
    }
}