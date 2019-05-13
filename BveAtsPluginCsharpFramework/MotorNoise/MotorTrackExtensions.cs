namespace AtsPlugin.MotorNoise
{
    public static class MotorTrackExtensions
    {
        public static AtsMotorNoise.MotorTrack FindByIndex(this AtsMotorNoise.MotorTrack[] self, int index)
        {
            foreach (var track in self)
            {
                if (track.Index != index)
                {
                    continue;
                }


                return track;
            }


            return null;
        }
    }
}
