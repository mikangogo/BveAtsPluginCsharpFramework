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

        private AtsVehicleState _vehicleState;
        private DoorStateType _doorState = DoorStateType.NotInitialized;

        public DoorStateType DoorState => _doorState;
        public double Location => _vehicleState.Location;
        public float Velocity => _vehicleState.Speed;
        public float AbsoluteVelocity => Math.Abs(_vehicleState.Speed);
        public float VelocityFromDeltaLocation { get; private set; }
        public float AbsoluteVelocityFromDeltaLocation => Math.Abs(VelocityFromDeltaLocation);
        public int SimulationTime => _vehicleState.Time;
        public float BcPressure => _vehicleState.BcPressure;
        public float MrPressure => _vehicleState.MrPressure;
        public float ErPressure => _vehicleState.ErPressure;
        public float BpPressure => _vehicleState.BpPressure;
        public float SapPressure => _vehicleState.SapPressure;
        public float MainCircuitCurrent => _vehicleState.Current;


        internal void SetVehicleState(AtsVehicleState vehicleState)
        {
            _vehicleState = vehicleState;
        }

        internal void SetDoorState(DoorStateType doorState)
        {
            _doorState = doorState;
        }

        internal void SetVelocityFromDeltaLocation(float velocity)
        {
            VelocityFromDeltaLocation = velocity;
        }

        internal void CopyFrom(AtsSimulationStates source)
        {
            _vehicleState = source._vehicleState;
            _doorState = source._doorState;
        }
    }
}
