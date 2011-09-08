using System.Text.RegularExpressions;
using System.Net.Mail;

namespace Utilities.General
{
    public class EmailHelper
    {
        public static void SendMail(string host, string to, string from, string cc,
                                    string subject, string content)
        {
            SendMail(host, to, from, cc, subject, content, null);
        }

        public static void SendMail(string host, string to, string from, string cc,
                                    string subject, string content, string[] attachments)
        {
                MailMessage msg = null;
                string[] toList = to.Split(',', ';');
                foreach (string s in toList)
                {
                    if (msg == null)
                    {
                        msg = new MailMessage(from, s);
                    }
                    else if (!string.IsNullOrEmpty(s))
                    {
                        msg.To.Add(s);
                    }
                }

                string body = ResourceHelper.GetResourceAsString(typeof(EmailHelper), "Utilities", "Resources", "EmailTemplate.txt");

                body = body.Replace("$TITLE$", subject);
                body = body.Replace("$BODY$", content);

                msg.Priority = MailPriority.Normal;
                msg.IsBodyHtml = true;
                msg.Subject = subject;
                msg.Body = body;
                if (!string.IsNullOrEmpty(cc))
                {
                    string[] ccList = cc.Split(',', ';');
                    foreach (string s in ccList)
                    {
                        msg.CC.Add(s);
                    }
                }

                if (attachments != null)
                {
                    foreach (string attachment in attachments)
                    {
                        msg.Attachments.Add(new Attachment(attachment));
                    }
                }

                var client = new SmtpClient(host);
                client.Send(msg);
        }

        public static bool IsValidAddress(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                             @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                             @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$").IsMatch(email);
        }

    }
}