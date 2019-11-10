using System;
using System.Diagnostics;

using SlimDX.DirectSound;

namespace AtsPlugin.Audio
{
    public static class AtsAudioManager
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

        public static void SetupAudio(AtsAudioTrack audioTrack)
        {
            DisposeActionDelegate += audioTrack.Dispose;
            audioTrack.SetupSoundBuffer(DirectSoundDevice);
        }

    }
}
