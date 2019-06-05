using System.Linq;
using System.Collections.Generic;

using AtsPlugin.Parametrics;

namespace AtsPlugin.MotorNoise
{
    public class AtsMotorNoiseTable
    {
        public class Track
        {
            public KeyValuePair<float, float>[] Pairs { get; private set; }


            public Track(KeyValuePair<float, float>[] pairs)
            {
                Pairs = pairs;
            }

            public float this[float x]
            {
                get
                {
                    if (Pairs.Length == 0)
                    {
                        return 0.0f;
                    }
                    else if (Pairs.Length == 1)
                    {
                        return Pairs[0].Value;
                    }
                    else if (x < Pairs[0].Key)
                    {
                        return 0.0f;
                    }


                    for (var i = 0; i < Pairs.Length - 1; ++i)
                    {
                        var x0 = Pairs[i].Key;
                        var x1 = Pairs[i + 1].Key;
                        var y0 = Pairs[i].Value;
                        var y1 = Pairs[i + 1].Value;

                        if ((x > x0) && (x > x1))
                        {
                            if (i != Pairs.Length - 2)
                            {
                                continue;
                            }
                        }

                        return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
                    }

                    return Pairs[Pairs.Length - 1].Value;
                }
            }
        }


        public Track[] Tracks { get; private set; }
        public Track this[int index] { get => Tracks[index]; }
        public int Length => Tracks.Length;


        public AtsMotorNoiseTable(AtsTable table)
        {
            Tracks = table.Tracks.Select(x => new Track(
                x.Pairs.Select(y => new KeyValuePair<float, float>(float.Parse(y.Key), float.Parse(y.Value)))
                .OrderBy(z => z.Key).ToArray()))
                .ToArray();
        }
    }
}
