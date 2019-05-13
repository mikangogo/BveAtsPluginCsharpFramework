using System.Collections.Generic;
using System.Linq;

namespace AtsPlugin.Parametrics
{
    public class AtsTable
    {
        public class Track
        {
            public KeyValuePair<string, string>[] Pairs { get; set; } = Enumerable.Empty< KeyValuePair<string, string>>().ToArray();

            public KeyValuePair<string, string> this[int index]
            {
                get
                {
                    return Pairs[index];
                }
            }

            public string this[string key]
            {
                get
                {
                    return Pairs.Single(e => e.Key == key).Value;
                }
            }

            public int Length { get { return Pairs.Length; } }


            public Track(KeyValuePair<string, string>[] pairs)
            {
                Pairs = pairs;
            }
        }


        public Track[] Tracks { get; private set; } = null;
        private string Path { get; set; }

        public Track this[int trackIndex]
        {
            get
            {
                return Tracks[trackIndex];
            }
        }

        public int Length { get { return Tracks.Length; } }

        public AtsTable(Track[] tracks, string path)
        {
            Tracks = tracks;
            Path = path;
        }
    }
}
