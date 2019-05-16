namespace AtsPlugin.MotorNoise
{
    public static class AtsMotorNoiseMotorTrackExtensions
    {
        public static void SetParameter(this AtsMotorNoise.MotorTrack[] self, float pitch, float volume)
        {
            foreach (var motorTrack in self)
            {
                motorTrack.Audio.Pitch = pitch;
                motorTrack.Audio.Volume = volume;
            }
        }

        public static void Update(this AtsMotorNoise.MotorTrack[] self)
        {
            foreach (var motorTrack in self)
            {
                motorTrack.Update();
            }
        }
    }
}
