using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Utilities.General
{
    public static class ResourceHelper
    {
        public static string GetResourceAsString(Type typeOfCallingObject, string namespaceName, string fileName)
        {
            return GetResourceAsString(typeOfCallingObject, string.Format("{0}.{1}", namespaceName, fileName));
        }

        public static string GetResourceAsString(Type typeOfCallingObject, string namespaceName, string folderName, string fileName)
        {
            return GetResourceAsString(typeOfCallingObject, string.Format("{0}.{1}.{2}", namespaceName, folderName, fileName));
        }

        public static string GetResourceAsString(Type typeOfCallingObject, string namespaceName, string[] folderPaths, string fileName)
        {
            var fullFolderPath = new StringBuilder();

            if (folderPaths != null && folderPaths.Count() > 0)
            {
                foreach (var folderPath in folderPaths)
                {
                    fullFolderPath.AppendFormat("{0}.", folderPath);
                }

                // Remove the trailing period
                fullFolderPath.Remove(fullFolderPath.Length - 1, 1);
            }

            return GetResourceAsString(typeOfCallingObject, string.Format("{0}.{1}.{2}", namespaceName, fullFolderPath, fileName));
        }

        public static string GetResourceAsString(Type typeOfCallingObject, string fullResourceName)
        {
            var fileStream = Assembly.GetAssembly(typeOfCallingObject).GetManifestResourceStream(fullResourceName);

            if (fileStream == null)
            {
                throw new Exception(string.Format("Resource '{0}' not found!", fullResourceName));
            }

            var sqlStreamReader = new StreamReader(fileStream);

            return sqlStreamReader.ReadToEnd();
        }

		public static byte[] GetResourceAsByteArray(Type typeOfCallingObject, string namespaceName, string fileName)
		{
			var fullResourceName = string.Format("{0}.{1}", namespaceName, fileName);

			var stream = Assembly.GetAssembly(typeOfCallingObject).GetManifestResourceStream(fullResourceName);

			if (stream == null)
			{
				throw new Exception(string.Format("Resource '{0}' not found!", fullResourceName));
			}

			var result = new byte[stream.Length];

			stream.Read(result, 0, result.Length);

			return result;
		}

		public static Stream GetResourceStream(Type typeOfCallingObject, string namespaceName, string fileName)
		{
			var fullResourceName = string.Format("{0}.{1}", namespaceName, fileName);

			var stream = Assembly.GetAssembly(typeOfCallingObject).GetManifestResourceStream(fullResourceName);

			if (stream == null)
			{
				throw new Exception(string.Format("Resource '{0}' not found!", fullResourceName));
			}

			return stream;
		}

		public static Stream GetResourceStream(Type typeOfCallingObject, string namespaceName, string[] folderPaths, string fileName)
		{
			var fullFolderPath = new StringBuilder();

			if (folderPaths != null && folderPaths.Count() > 0)
			{
				foreach (var folderPath in folderPaths)
				{
					fullFolderPath.AppendFormat("{0}.", folderPath);
				}

				// Remove the trailing period
				fullFolderPath.Remove(fullFolderPath.Length - 1, 1);
			}

			var fullNamespacePath = string.Format("{0}.{1}", namespaceName, fullFolderPath);

			return GetResourceStream(typeOfCallingObject, fullNamespacePath, fileName);
		}

    	public static string[] GetAllResourcesFromAssemblyContaining(Type type)
    	{
    		return type.Assembly.GetManifestResourceNames();
    	}
    }

    public class ResourceDeclaration
    {
        public string[] FolderPaths { get; set; }
        public string ResourceName { get; set; }
        protected Dictionary<string, object> Tokens { get; set; }

        public ResourceDeclaration() : this(new string[] {}) {}

        public ResourceDeclaration(IEnumerable<string> tokenKeys)
        {
            Tokens = new Dictionary<string, object>();

            foreach (var key in tokenKeys)
            {
                Tokens.Add(key, "");
            }
        }

        public ResourceDeclaration SetTokenValue(string key, object value)
        {
            if(!Tokens.Keys.Contains(key))
            {
                throw new ArgumentException("Token not defined on declaration definition!", key);
            }

            Tokens[key] = value;

            return this;
        }

        public string ReplaceTokens(string tokenedString)
        {
            foreach (var token in Tokens)
            {
                tokenedString = tokenedString.Replace(token.Key, token.Value.ToString());
            }

            return tokenedString;
        }
    }
}