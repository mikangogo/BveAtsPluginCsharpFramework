using System;
using System.Collections.Generic;
using AtsPlugin.Parametrics;
using System.Linq;

namespace AtsPlugin.Importing
{
    public static partial class AtsParser
    {
        public static AtsTable ParseTable(string path, Func<string, string> readFunction)
        {
            return ParseTable(readFunction(path), path);
        }

        public static AtsTable ParseTable(string body, string path = null)
        {
            path = string.IsNullOrEmpty(path) ? string.Empty : path;

            var keyValueDictionary = new Dictionary<string, string[]>();


            Action<string> parseLine = (line) =>
            {
                var lineValues = line.Split(',');


                if ((lineValues.Length == 0) || string.IsNullOrEmpty(lineValues[0]))
                {
                    // Invalid line. Skip parsing.
                    return;
                }


                var key = lineValues[0];
                var sourceValues = (lineValues.Length >= 1) ? lineValues.Where((e, i) => i >= 1).ToArray() : Enumerable.Empty<string>().ToArray();


                if (keyValueDictionary.ContainsKey(key))
                {
                    // Already exists the key.

                    var destValues = keyValueDictionary[key];
                    var isDestLessThanSource = destValues.Length < sourceValues.Length;
                    

                    if (isDestLessThanSource)
                    {
                        // sourceValues.Length < destValues.Length

                        destValues = destValues.Select((e, i) => string.IsNullOrEmpty(destValues[i]) ? sourceValues[i] : e).ToArray();
                    }
                    else
                    {
                        // sourceValues.Length >= destValues.Length

                        destValues = sourceValues.Select((e, i) => string.IsNullOrEmpty(destValues[i]) ? e : destValues[i]).ToArray();
                    }

                    keyValueDictionary[key] = destValues;
                }
                else
                {
                    // Register the new key.

                    keyValueDictionary.Add(key, sourceValues);
                }
            };


            Parse(body, parseLine);


            var requiredTrackCount = keyValueDictionary.Max(e => e.Value.Length);
            var tracks = new AtsTable.Track[requiredTrackCount];


            tracks = tracks.Select((x, i) => new AtsTable.Track(
                keyValueDictionary.Where(e => ((i < e.Value.Length) && (!string.IsNullOrEmpty(e.Value[i]))))
                .Select(e => new KeyValuePair<string, string>(e.Key, e.Value[i]))
                .ToArray()))
                .ToArray();

            
            return new AtsTable(tracks, path);
        }
    }
}
