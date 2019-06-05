using System.Collections.Generic;
using System.Linq;

namespace AtsPlugin.Parametrics
{
    public class AtsTable
    {
        public class Track
        {
            public KeyValuePair<string, string>[] Pairs { get; set; } = Enumerable.Empty< KeyValuePair<string, string>>().ToArray();
            public KeyValuePair<string, string> this[int index] => Pairs[index];
            public string this[string key] { get => Pairs.Single(e => e.Key == key).Value; }
            public int Length => Pairs.Length;


            public Track(KeyValuePair<string, string>[] pairs)
            {
                Pairs = pairs;
            }
        }


        private string Path { get; set; }

        public Track[] Tracks { get; private set; } = null;
        public Track this[int trackIndex] { get => Tracks[trackIndex]; }
        public int Length { get => Tracks.Length; }


        public AtsTable(Track[] tracks, string path)
        {
            Tracks = tracks;
            Path = path;
        }
    }
}
