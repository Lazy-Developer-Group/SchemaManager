namespace Utilities.General
{
	public interface ISendEmails
	{
		void SendMail(string host, string to, string from, string cc, string subject, string content);

		void SendMail(string host, string to, string from, string cc, string subject, string content, string[] attachments);
	}
}
