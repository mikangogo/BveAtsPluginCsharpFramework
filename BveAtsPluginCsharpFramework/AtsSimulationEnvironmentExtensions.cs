namespace AtsPlugin
{
    public static class AtsSimulationEnvironmentExtensions
    {
        public static bool IsPressedKey(this AtsSimulationEnvironment self, AtsKey keyType)
        {
            return self.CurrentKeyStates[keyType].IsDown;
        }

        public static bool IsTriggeredKey(this AtsSimulationEnvironment self, AtsKey keyType)
        {
            return (self.CurrentKeyStates[keyType].IsDown && self.LastKeyStates[keyType].IsUp);
        }

        public static bool IsReleasedKey(this AtsSimulationEnvironment self, AtsKey keyType)
        {
            return (self.CurrentKeyStates[keyType].IsUp && self.LastKeyStates[keyType].IsDown);
        }

        public static void UpdateVelocityFromDeltaLocation(this AtsSimulationEnvironment self)
        {
            var deltaLocation = self.CurrentStates.Location - self.LastStates.Location;
            var deltaTime = (double)(self.CurrentStates.SimulationTime - self.LastStates.SimulationTime);

            if (deltaTime > 0.0)
            {
                var kmph = deltaLocation / deltaTime * 3600.0;
                self.CurrentStates.SetVelocityFromDeltaLocation((float)(kmph));
            }
        }
    }
}
