namespace AtsPlugin
{
    public interface IAtsBehaviour
    {
        void Awake(AtsSimulationEnvironment environment);
        void OnDestroy();

        void OnActivate();
        void OnStart(AtsInitialHandlePosition initialHandlePosition);

        void OnDoorStateChanged(AtsSimulationStates.DoorStateType doorState);
        void OnHornBlew(AtsHornType hornType);
        void OnControlHandleMoved(int position, AtsSimulationEnvironment.ControlHandleType controlHandleType);
        void OnSignalChanged(int signalIndex);
        void OnBeaconDataReceived(AtsBeaconData beaconData);

        void Update();
    }
}
