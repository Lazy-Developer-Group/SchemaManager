using System;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;

namespace Utilities.General
{
    public static class ZipHelper
    {
		public static void UnzipSingle(string zipFilePath, string destinationFilePath)
		{
			using (var fileStream = File.OpenRead(zipFilePath))
			{
				using (var zipFile = new ZipFile(fileStream))
				{
					if (zipFile.Count > 1)
					{
						throw new InvalidOperationException("Attempted to unzip an archive containing multiple files to a single destination file.  Use Unzip instead.");
					}

					var entry = zipFile.Cast<ZipEntry>().First();

					byte[] buffer = new byte[4096];		// 4K is optimum
					var zipStream = zipFile.GetInputStream(entry);

					using (var streamWriter = File.Create(destinationFilePath))
					{
						StreamUtils.Copy(zipStream, streamWriter, buffer);
					}
				}
			}
		}

        public static void Unzip(string zipFilePath, string destinationDirectory)
        {
            ZipFile zipFile = null;
            try
            {
                var fileStream = File.OpenRead(zipFilePath);
                zipFile = new ZipFile(fileStream);

                foreach (ZipEntry zipEntry in zipFile)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;			// Ignore directories
                    }

                    byte[] buffer = new byte[4096];		// 4K is optimum
                    var zipStream = zipFile.GetInputStream(zipEntry);

                    string fullZipToPath = Path.Combine(destinationDirectory, zipEntry.Name);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    using (var streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zipFile != null)
                {
                    zipFile.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zipFile.Close(); // Ensure we release resources
                }
            }

        }
    }
}
