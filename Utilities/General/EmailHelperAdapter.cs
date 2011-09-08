namespace Utilities.General
{
	public class EmailHelperAdapter : ISendEmails
	{
		public void SendMail(string host, string to, string from, string cc, string subject, string content)
		{
			EmailHelper.SendMail(host, to, from, cc, subject, content);
		}

		public void SendMail(string host, string to, string from, string cc, string subject, string content, string[] attachments)
		{
			EmailHelper.SendMail(host, to, from, cc, subject, content, attachments);
		}
	}
}
