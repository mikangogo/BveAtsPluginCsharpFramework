using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AtsPlugin.Importing
{
    public static class AtsStorage
    {
        public static string ReadText(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(AtsModule.ModuleDirectoryPath, path);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{typeof(AtsStorage).Name}: The file does not exist: {path}");
            }

            AtsDebug.LogInfo($"{nameof(AtsStorage)}: ReadText: Load: {path}");


            var header = string.Empty;
            var body = string.Empty;
            var encoding = Encoding.UTF8;
            var hasHeader = false;
            

            using (var streamReader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                header = streamReader.ReadLine();
            }


            var match = Regex.Match(header.ToLower(), "bvets.+|version .+");

            if (match.Success)
            {
                hasHeader = true;

                match = Regex.Match(header.ToLower(), ":");

                if (match.Success)
                {
                    var headerLength = header.IndexOf(',');
                    var encodingName = (headerLength == -1 ? header : header.Substring(0, headerLength)).Substring(match.Index + 1);
                    encoding = Encoding.GetEncoding(encodingName);
                }
            }


            using (var streamReader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), encoding))
            {
                if (hasHeader)
                {
                    streamReader.ReadLine();
                }

                body = streamReader.ReadToEnd();
            }


            return body;
        }
    }
}
