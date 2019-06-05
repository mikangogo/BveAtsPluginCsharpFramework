using System;
using System.Diagnostics;
using System.IO;

using SlimDX.DirectSound;
using SlimDX.Multimedia;

using AtsPlugin.Parametrics;

namespace AtsPlugin.MotorNoise
{
    public class AtsMotorNoise
    {
        private static Action DisposeActionDelegate { get; set; }

        private static DirectSound DirectSoundDevice { get; set; } = null;
        private static PrimarySoundBuffer PrimaryBuffer { get; set; } = null;
        private static SoundBufferDescription PrimaryBufferDesc { get; set; }


        public static void Startup()
        {
            DirectSoundDevice = new DirectSound();


            if (DirectSoundDevice.SetCooperativeLevel(Process.GetCurrentProcess().MainWindowHandle, CooperativeLevel.Priority).IsFailure)
            {
                throw new InvalidOperationException("Initializing DirectSound was failed.");
            }


            PrimaryBufferDesc = new SoundBufferDescription
            {
                Format = null,
                Flags = BufferFlags.PrimaryBuffer,
                SizeInBytes = 0,
            };


            PrimaryBuffer = new PrimarySoundBuffer(DirectSoundDevice, PrimaryBufferDesc);
            PrimaryBuffer.Play(0, PlayFlags.Looping);
        }

        public static void Cleanup()
        {
            // HACK: `DirectSound.Dispose()` calls Thread.Join() in itself.
            // Bve doesn't wait the running thread object when disposing the plug-in.
            // We have to block disposing forcibly.
            var disposeThread = new System.Threading.Thread(DisposeThread);
            disposeThread.Start();


            var beginTime = DateTime.Now.Millisecond;

            while (disposeThread.ThreadState == System.Threading.ThreadState.Unstarted)
            {
                if ((DateTime.Now.Millisecond - beginTime) > 5000)
                {
                    // Timed out.
                    break;
                }
            }


            beginTime = DateTime.Now.Millisecond;

            while (disposeThread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                // Busy loop.
                // Forcibly waiting.

                if ((DateTime.Now.Millisecond - beginTime) > 5000)
                {
                    // Timed out.
                    break;
                }
            }
        }

        private static void DisposeThread()
        {
            DisposeActionDelegate?.Invoke();
            PrimaryBuffer.Dispose();
            DirectSoundDevice.Dispose();


            DisposeActionDelegate = null;
            PrimaryBuffer = null;
            DirectSoundDevice = null;
        }


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

        public class MotorAudio : IDisposable
        {
            private byte[] RawAudioStream { get; set; } = null;
            private SecondarySoundBuffer SecondaryBuffer { get; set; } = null;
            private SoundBufferDescription SecondaryBufferDesc { get; set; }

            private int MinimumFrequency { get; set; } = 0;
            private int MaximumFrequency { get; set; } = 0;

            private int DefaultFrequency { get; set; } = 0;
            private readonly int MinimumVolume = -10000;

            public float Pitch { set; get; }
            public float Volume { set; get; }

            public MotorAudio(Stream stream)
            {
                using (var audioStream = new WaveStream(stream))
                {
                    SecondaryBufferDesc = new SoundBufferDescription
                    {
                        Format = audioStream.Format,
                        Flags = BufferFlags.ControlVolume | BufferFlags.ControlFrequency | BufferFlags.ControlPan | BufferFlags.GetCurrentPosition2,
                        SizeInBytes = (int)audioStream.Length
                    };


                    RawAudioStream = new byte[SecondaryBufferDesc.SizeInBytes];

                    audioStream.Read(RawAudioStream, 0, (int)audioStream.Length);
                }
            }
            
            public void SetupSoundBuffer(DirectSound device)
            {
                if (SecondaryBuffer != null)
                {
                    throw new InvalidOperationException("Already exists SoundBuffer.");
                }

                SecondaryBuffer = new SecondarySoundBuffer(device, SecondaryBufferDesc);
                SecondaryBuffer.Write(RawAudioStream, 0, LockFlags.EntireBuffer);
                SecondaryBuffer.CurrentPlayPosition = 0;
                SecondaryBuffer.Volume = MinimumVolume;

                MinimumFrequency = device.Capabilities.MinSecondarySampleRate;
                MaximumFrequency = device.Capabilities.MaxSecondarySampleRate;

                DefaultFrequency = SecondaryBufferDesc.Format.SamplesPerSecond;
            }

            public void Update()
            {
                var pitch = Math.Max(Pitch, 0.0f);
                var volume = Math.Max(Math.Min(Volume, 1.0f), 0.0f);

                SecondaryBuffer.Frequency = Math.Min(Math.Max((int)(DefaultFrequency * pitch), MinimumFrequency), MaximumFrequency);

                var gain = MinimumVolume;


                if (volume > 0.001f)
                {
                    var attenuation = volume;


                    if (attenuation >= 1.0f)
                    {
                        attenuation = 1.0f;
                    }


                    gain = (int)(Math.Log10(1.0 / 1024.0 + attenuation / 1.0 * 1023.0 / 1024.0) * 2000.0);
                }


                SecondaryBuffer.Volume = gain;


                if (volume > 0.0f)
                {
                    if ((SecondaryBuffer.Status & BufferStatus.Playing) != 0)
                    {
                        return;
                    }


                    SecondaryBuffer.CurrentPlayPosition = 0;
                    SecondaryBuffer.Play(0, PlayFlags.Looping);


                    return;
                }


                if (SecondaryBuffer.Status == BufferStatus.None)
                {
                    return;
                }


                SecondaryBuffer.Stop();
            }

            public void Dispose()
            {
                SecondaryBuffer.Dispose();
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

            public void SetupAudio(DirectSound device)
            {
                Audio.SetupSoundBuffer(device);
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


            foreach (var motorTrack in motorTracks)
            {
                DisposeActionDelegate += motorTrack.Dispose;
                motorTrack.SetupAudio(DirectSoundDevice);
            }
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
