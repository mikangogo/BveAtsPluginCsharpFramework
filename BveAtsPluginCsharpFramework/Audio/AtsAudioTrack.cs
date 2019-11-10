using System;
using System.IO;

using SlimDX.DirectSound;
using SlimDX.Multimedia;


namespace AtsPlugin.Audio
{
    public class AtsAudioTrack : IDisposable
    {
        public enum PlayingState
        {
            Stop,
            Pause,
            Play,
        }

        private byte[] RawAudioStream { get; set; } = null;
        protected SecondarySoundBuffer SecondaryBuffer { get; private set; } = null;
        protected SoundBufferDescription SecondaryBufferDesc { get; private set; }

        private int MinimumFrequency { get; set; } = 0;
        private int MaximumFrequency { get; set; } = 0;

        private int DefaultFrequency { get; set; } = 0;
        private readonly int MinimumVolume = -10000;

        private PlayingState _playingState = PlayingState.Stop;

        public float Pitch { set; get; }
        public float Volume { set; get; }
        public PlayingState PlayState { 
            get { return _playingState; }
            set 
            {
                switch (value)
                {
                    case PlayingState.Stop:
                        SecondaryBuffer.CurrentPlayPosition = 0;
                        SecondaryBuffer.Stop();
                        break;

                    case PlayingState.Pause:
                        SecondaryBuffer.Stop();
                        break;

                    case PlayingState.Play:
                        SecondaryBuffer.Play(0, IsLooped ? PlayFlags.Looping : PlayFlags.None);
                        break;

                    default:
                        return;
                }

                _playingState = value;
            } 
        }
        public bool IsLooped { get; set; }

        public static AtsAudioTrack LoadFromFile(string path)
        {
            return new AtsAudioTrack(new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)).BaseStream);
        }

        public AtsAudioTrack(Stream stream)
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


            AtsAudioManager.SetupAudio(this);
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


            OnUpdate();

        }

        public void Dispose()
        {
            SecondaryBuffer.Dispose();
        }

        protected virtual void OnUpdate()
        {
            if (!IsLooped)
            {
                if ((SecondaryBuffer.Status & BufferStatus.Playing) != 0)
                {
                    return;
                }

                if (PlayState == PlayingState.Pause)
                {
                    return;
                }


                PlayState = PlayingState.Stop;

                return;
            }
        }
    }
}
