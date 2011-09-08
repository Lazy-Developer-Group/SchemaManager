using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace Utilities.Web
{
    public class WebLog
    {
        private static object _lock = new object();

        /// <summary>       
        /// Given a string of data, this method will produce ASCII-only output. All
        /// non-printable chars, such as newlines and tabs, will be converted to
        /// hex format.<br/>
        /// <code>
        /// So given the following input (\n is a newline):
        /// This is a\ntest
        ///
        /// you will get back:
        /// This is a\x0Atest
        /// </code>
        /// </summary>
        /// <param name="entry">The entry to clean</param>
        /// <returns>A clean log entry</returns>
        public static string CleanEntry(string entry)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < entry.Length; i++)
            {
                char c = entry[i];

                if ((int)c < 32 || (int)c > 126)
                {
                    sb.Append("\\x" + string.Format("{0:X2}", (int)c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Writes a message to the underlying fileStream.  If the level is LoggingLevel.None,
        /// then this is a no-op. All non-printable characters are replaced with their hex equivalents.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void WriteEntry(LoggingLevel level, string message)
        {
            WriteEntry(level, message, null);
        }

        /// <summary>
        /// Writes a message to the underlying fileStream.  If the level is LoggingLevel.None,
        /// then this is a no-op. All non-printable characters are replaced with their hex equivalents.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public static void WriteEntry(LoggingLevel level, string message, Exception e)
        {

            if (level == LoggingLevel.None)
                return;

            lock (_lock)
            {
                FileStream fs = null;

                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(DateTime.Now.ToString());
                    sb.Append('\t');
                    sb.Append(level.ToString());
                    sb.Append('\t');
                    sb.Append(CleanEntry(message));
                    sb.Append('\t');

                    if (e != null)
                    {
                        sb.Append(CleanEntry(e.ToString()));
                    }

                    sb.Append("\r\n");

                    byte[] buf = Encoding.ASCII.GetBytes(sb.ToString());

                    string baseLogDir = ConfigurationManager.AppSettings["LogDirectory"];

                    fs = GetLogStream(baseLogDir, string.Format("{0}.txt", DateTime.Now.ToString("MMddyyyy")));
                    fs.Write(buf, 0, buf.Length);

                }
                catch (Exception)
                {
                    /*nothing*/
                }
                finally
                {
                    CloseStream(fs);
                }
            }
        }

        /// <summary>
        /// Closes the fileStream quietly. 
        /// </summary>
        /// <param name="fileStream"></param>
        protected static void CloseStream(FileStream fileStream)
        {
            try
            {
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (Exception)
            {
                /*nothing*/
            }
        }

        /// <summary>
        /// Returns a stream to the log file.  If the file exists we append, otherwise
        /// we create a new file.  If the directory does not exist, then we create that
        /// as well.
        /// </summary>
        /// <param name="dir">The directory where the log file resides</param>
        /// <param name="fileName">The name of the log file</param>
        /// <returns>A filestream to log with</returns>
        protected static FileStream GetLogStream(string dir, string fileName)
        {

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string location = dir.EndsWith("\\") ? dir + fileName : dir + "\\" + fileName;

            if (File.Exists(location))
            {
                return File.Open(location, FileMode.Append);
            }
            else
            {
                return File.Create(location, 1024);
            }
        }
    }

    /// <summary>
    /// A simple enumerations that puts structure aroung log levels.
    /// </summary>
    public enum LoggingLevel
    {
        /// <summary>
        /// No logging
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates debug level or higher
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Indicates info level or higher
        /// </summary>
        Info = 2,
        /// <summary>
        /// Indicates warn level or higher
        /// </summary>
        Warn = 3,
        /// <summary>
        /// Indicates error level
        /// </summary>
        Error = 4
    }
}