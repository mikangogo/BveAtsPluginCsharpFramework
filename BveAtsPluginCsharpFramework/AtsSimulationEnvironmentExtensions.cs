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
    }
}
