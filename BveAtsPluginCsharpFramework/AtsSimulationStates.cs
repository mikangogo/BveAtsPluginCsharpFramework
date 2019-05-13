using System;

namespace AtsPlugin
{
    [Serializable]
    public class AtsSimulationStates
    {
        [Serializable]
        public enum DoorStateType
        {
            NotInitialized,
            Open,
            Close
        }

        public DoorStateType DoorState { get => _doorState; }
        public double Location { get => _vehicleState.Location; }
        public float Velocity { get => _vehicleState.Speed; }
        public float AbsoluteVelocity { get => Math.Abs(_vehicleState.Speed); }
        public int SimulationTime { get => _vehicleState.Time; }
        public float BcPressure { get => _vehicleState.BcPressure; }
        public float MrPressure { get => _vehicleState.MrPressure; }
        public float ErPressure { get => _vehicleState.ErPressure; }
        public float BpPressure { get => _vehicleState.BpPressure; }
        public float SapPressure { get => _vehicleState.SapPressure; }
        public float MainCircuitCurrent { get => _vehicleState.Current; }

        private AtsVehicleState _vehicleState;
        private DoorStateType _doorState = DoorStateType.NotInitialized;

        internal void SetVehicleState(AtsVehicleState vehicleState)
        {
            _vehicleState = vehicleState;
        }

        internal void SetDoorState(DoorStateType doorState)
        {
            _doorState = doorState;
        }

        internal void CopyFrom(AtsSimulationStates source)
        {
            _vehicleState = source._vehicleState;
            _doorState = source._doorState;
        }
    }
}
