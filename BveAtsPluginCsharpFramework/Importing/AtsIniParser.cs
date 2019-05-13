using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using AtsPlugin.Parametrics;

namespace AtsPlugin.Importing
{
    public static partial class AtsParser
    {
        public static AtsIni ParseIni(string path, Func<string, string> readFunction)
        {
            return ParseIni(readFunction(path), path);
        }

        public static AtsIni ParseIni(string body, string path = null)
        {
            path = string.IsNullOrEmpty(path) ? string.Empty : path;

            var sections = new Dictionary<string, AtsIni.Section>();
            var sectionName = string.Empty;
            var pairs = new Dictionary<string, string>();

            Action<string> parseLine = (line) =>
            {
                char capital = line[0];


                if (capital == '[')
                {
                    if (!string.IsNullOrEmpty(sectionName))
                    {
                        sections.Add(sectionName, new AtsIni.Section(sectionName, pairs, path));
                    }

                    pairs = new Dictionary<string, string>();

                    sectionName = Regex.Replace(line, @"[\[|\]]", "").ToLower();


                    return;
                }


                var delimIndex = line.IndexOf('=');
                var key = line.Substring(0, delimIndex).ToLower();
                var valueStartIndex = delimIndex + 1;
                var value = valueStartIndex < line.Length ? line.Substring(valueStartIndex) : string.Empty;


                pairs.Add(key, value);
            };


            Parse(body, parseLine);


            if (!string.IsNullOrEmpty(sectionName))
            {
                sections.Add(sectionName, new AtsIni.Section(sectionName, pairs, path));
            }


            return new AtsIni(sections, path);
        }
    }
}
