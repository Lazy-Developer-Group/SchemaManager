using System.IO;
using System.Linq;

namespace Utilities.General
{
    public static class DirectoryInfoExtensions
    {
        public static FileInfo[] GetFilesSortedByLastWriteTime(this DirectoryInfo directoryInfo)
        {
            var files = from f in directoryInfo.GetFiles()
                        orderby f.LastWriteTime
                        select f;

            return files.ToArray();     
        }
    }
}
