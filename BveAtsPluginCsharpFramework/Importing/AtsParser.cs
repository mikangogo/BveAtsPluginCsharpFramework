using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AtsPlugin.Importing
{
    public static partial class AtsParser
    {
        public static void Parse(string body, Action<string> parseLine)
        {
            using (var stringReader = new StringReader(body))
            {
                while (stringReader.Peek() != -1)
                {
                    var line = Trim(stringReader.ReadLine());


                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }


                    parseLine(line);
                }
            }
        }


        private static string Trim(string line)
        {
            // Strip the comment.
            var match = Regex.Match(line, @"[#|;].*");

            if (match.Success)
            {
                if (match.Index == 0)
                {
                    return string.Empty;
                }


                line = line.Remove(match.Index);
            }


            // Detect .ini style.
            match = Regex.Match(line, @"=");

            if (match.Success)
            {
                // Strip the meaningless characters partially.
                var key = Regex.Replace(line, @"[\t| ]", "");
                var keyDelimiterIndex = key.IndexOf('=');
                key = key.Substring(0, keyDelimiterIndex + 1);      // Include '=' delimiter.

                line = key + line.Substring(match.Index + match.Length).TrimStart();            // The result is "Key=Value".
            }
            else
            {
                // Strip the meaningless characters in all.
                line = Regex.Replace(line, @"[\t| ]", "");
            }


            return line;
        }
    }
}
