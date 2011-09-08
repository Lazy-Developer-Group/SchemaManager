namespace Utilities.General
{
    public static class StringExtensions
    {
        public static string NewlinesToHTMLBreaks(this string original)
        {
            return original.Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>");
        }

		public static string TrimTo(this string value, int maxCharacters, string suffix)
		{
			//Only truncate the value if the truncated value including the suffix is less than 
			//the max length
			return value.Length > (maxCharacters + suffix.Length) ? value.Substring(0, maxCharacters) + suffix : value;
		}
    }
}
