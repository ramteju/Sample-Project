using Entities;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Common
{
    public static class HandleException
    {
        static string FormatException(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Current User : " + Environment.UserName);
            sb.AppendLine();
            sb.Append("System : " + Environment.MachineName);
            sb.AppendLine();
            sb.Append("Message : " + ex.Message);
            sb.AppendLine();
            sb.Append("Source : " + ex.Source);
            sb.AppendLine();
            sb.Append("Method : " + ex.TargetSite?.Name);
            sb.AppendLine();
            sb.Append("HResult : " + ex.HResult);
            sb.AppendLine();
            sb.Append("Helplink : " + ex.HelpLink);
            sb.AppendLine();
            if (ex.InnerException != null)
            {
                sb.Append("Inner Exception : " + ex.InnerException.Message);
                sb.AppendLine();
            }
            if (ex.GetBaseException() != null)
            {
                sb.Append("Base Exception : " + ex.GetBaseException().Message);
                sb.AppendLine();
            }
            if (ex.GetType() != null)
            {
                sb.Append("Run Time Type : " + ex.GetType().Name);
                sb.AppendLine();
            }
            sb.Append("Strack Trace : ");
            sb.AppendLine();
            sb.Append(ex.StackTrace);
            sb.AppendLine();
            return sb.ToString();
        }

        //public static void AddException(Exception ex, int tanNumber, bool showError = true)
        //{
        //    string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"ApILog//{System.DateTime.Now.ToString("yyyy-MM-dd")}.Reactions.log");
        //    if (showError)
        //        MessageBox.Show("There is an error occured in application, the Application will exit now. The error will be reported to IT Team through Mail.");
        //    StringBuilder sb = new StringBuilder();
        //    string version = string.Empty;
        //    if (ApplicationDeployment.IsNetworkDeployed)
        //        version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4);
        //    else
        //        version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        //    sb.AppendLine("Application Version : " + version);
        //    sb.AppendLine("System Time : " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
        //    sb.AppendLine("Current TAN : " + tanNumber);
        //    sb.AppendLine(FormatException(ex));

        //    sb.AppendLine("Last few lines of log file . .");
        //    try
        //    {
        //        if (File.Exists(LogFile))
        //        {
        //            var lastLines = File.ReadLines(LogFile).Reverse().Take(50);
        //            sb.AppendLine(String.Join("\n", lastLines));
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        sb.AppendLine("Failed to read log file . . ." + x.Message);
        //    }

        //    sb.AppendLine();

        //    try
        //    {
        //        //Mail.sendMail(sb.ToString());
        //        ActivityTracing trace = new ActivityTracing();
        //        trace.ErrorMessage = sb.ToString();
        //        trace.User = Environment.UserName;
        //        trace.tanId = tanNumber;
        //        trace.MethodName = ex.TargetSite?.Name;
        //        trace.ToTime = trace.FromTime;
        //        trace.FromTool = true;
        //        RestHub.SaveException(trace).ContinueWith(c => { });
        //        sendMail(sb.ToString());
        //    }
        //    catch (Exception mailException)
        //    {
        //        string msg = "Sorry, Unfortunately, Error Reporting Failed ! Please copy the below message and mail to IT Team. Thanks for your patience\n" + sb.ToString() + "\n\n" + mailException.ToString();
        //        MessageBox.Show(msg, "Use CTRL+C to copy this error message . . .");
        //    }
        //}

        public static void sendMail(string mailBody)
        {
            string subject = "Reaction Issue - " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "SmtpAuth.GVKBIO.COM";
            client.Credentials = new System.Net.NetworkCredential("it.dev", "it#123");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("it.dev@gvkbio.com", "IT Development");
            //foreach (string toAddress in toMailIds)
                mailMessage.To.Add("ramu.ankam@excelra.com");
            //foreach (string ccAddress in ccMailIds)
               // if (!String.IsNullOrEmpty(ccAddress))
                    //mailMessage.CC.Add(ccAddress);
            mailMessage.Subject = subject;
            mailMessage.Body = mailBody;

            mailMessage.BodyEncoding = UTF8Encoding.UTF8;
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(mailMessage);
        }
    }
}
