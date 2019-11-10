using System;
using System.IO;

using SlimDX.DirectSound;

using AtsPlugin.Parametrics;
using AtsPlugin.Audio;

namespace AtsPlugin.MotorNoise
{
    public class AtsMotorNoise
    {
        public class ParameterTables
        {
            public AtsMotorNoiseTable Pitch { get; private set; } = null;
            public AtsMotorNoiseTable Volume { get; private set; } = null;

            
            public ParameterTables(AtsTable pitch, AtsTable volume)
            {
                Pitch = new AtsMotorNoiseTable(pitch);
                Volume = new AtsMotorNoiseTable(volume);
            }
        }

        public class MotorAudio : AtsAudioTrack
        {
            public MotorAudio(Stream stream) : base(stream)
            {
                IsLooped = true;
            }

            protected override void OnUpdate()
            {
                if (Volume > 0.0f)
                {
                    if ((SecondaryBuffer.Status & BufferStatus.Playing) != 0)
                    {
                        return;
                    }


                    SecondaryBuffer.CurrentPlayPosition = 0;
                    PlayState = PlayingState.Play;


                    return;
                }


                if (SecondaryBuffer.Status == BufferStatus.None)
                {
                    return;
                }


                PlayState = PlayingState.Stop;
            }
        }

        public class MotorTrack : IDisposable
        {
            public int Index { get; private set; } = -1;
            public string FilePath { get; private set; } = string.Empty;
            public MotorAudio Audio { get; private set; } = null;


            public MotorTrack(int index, string filePath, MotorAudio audio)
            {
                Index = index;
                FilePath = filePath;
                Audio = audio;
            }


            public void Update()
            {
                Audio.Update();
            }

            public void Dispose()
            {
                Audio.Dispose();
            }
        }


        public ParameterTables PositiveDirectionParameters { get; private set; } = null;
        public ParameterTables NegativeDirectionParameters { get; private set; } = null;
        public MotorTrack[] MotorTracks { get; private set; } = null;
        public float Volume { get; set; } = 1.0f;
        public float Position { get; set; } = 0.0f;
        public float DirectionMixtureRatio { get; set; } = 1.0f;


        public AtsMotorNoise(ParameterTables positiveDirectionParameters, ParameterTables negativeDirectionParameters)
        {
            PositiveDirectionParameters = positiveDirectionParameters;
            NegativeDirectionParameters = negativeDirectionParameters;
        }

        public void SetMotorTracks(MotorTrack[] motorTracks)
        {
            MotorTracks = motorTracks;
        }

        public void Update()
        {
            var absolutePosition = Math.Abs(Position);
            var mixtureRatio = Math.Max(Math.Min(DirectionMixtureRatio, 1.0f), 0.0f);


            MotorTracks.SetParameter(0.0f, 0.0f);


            EvaluatePitch(absolutePosition, mixtureRatio, PositiveDirectionParameters);
            EvaluateVolume(absolutePosition, mixtureRatio, PositiveDirectionParameters);
            EvaluatePitch(absolutePosition, 1.0f - mixtureRatio, NegativeDirectionParameters);
            EvaluateVolume(absolutePosition, 1.0f - mixtureRatio, NegativeDirectionParameters);


            MotorTracks.Update();
        }
        
        private void EvaluatePitch(float x, float mixtureRatio, ParameterTables table)
        {
            for (var i = 0; i < table.Pitch.Tracks.Length; ++i)
            {
                var pitch = table.Pitch.Tracks[i];
                var motorTrack = MotorTracks.FindByIndex(i);


                if (motorTrack == null)
                {
                    continue;
                }


                motorTrack.Audio.Pitch += pitch[x] * mixtureRatio;
            }
        }

        private void EvaluateVolume(float x, float mixtureRatio, ParameterTables table)
        {
            for (var i = 0; i < table.Volume.Tracks.Length; ++i)
            {
                var volume = table.Volume.Tracks[i];
                var motorTrack = MotorTracks.FindByIndex(i);


                if (motorTrack == null)
                {
                    continue;
                }
                

                motorTrack.Audio.Volume += (volume[x] * mixtureRatio) * Volume;
            }
        }
    }
}
