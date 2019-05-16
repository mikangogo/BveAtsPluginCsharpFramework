using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace AtsPlugin
{
    public sealed class AtsSimulationEnvironment
    {
        public enum ControlHandleType
        {
            Power,
            Brake,
            Reverser
        }

        public readonly int LowestPriorty = 1000000;

        public static AtsSimulationEnvironment Instance { get; } = new AtsSimulationEnvironment();

        public AtsBus Bus { get; private set; } = new AtsBus();
        public AtsNamedIoArray PanelOperations { get; private set; } = new AtsNamedIoArray();
        public AtsNamedIoArray SoundOperations { get; private set; } = new AtsNamedIoArray();
        public AtsControlHandle ControlHandle { get; private set; } = null;
        public AtsSimulationStates LastStates { get; private set; } = new AtsSimulationStates();
        public AtsSimulationStates CurrentStates { get; private set; } = new AtsSimulationStates();
        public AtsKeyStates LastKeyStates { get; private set; } = new AtsKeyStates();
        public AtsKeyStates CurrentKeyStates { get; private set; } = new AtsKeyStates();


        public double DeltaTime
        {
            get
            {
                return Math.Max(1.0, CurrentStates.SimulationTime - LastStates.SimulationTime);
            }
        }

        public float DeltaTimeF
        {
            get
            {
                return (float)DeltaTime;
            }
        }

        private IAtsBehaviour[] Behaviours { get; set; } = null;

        private AtsSimulationEnvironment()
        {
        }
        
        internal void OnLoad()
        {
            var behaviourList = new List<IAtsBehaviour>();


            var types = Assembly.GetAssembly(typeof(IAtsBehaviour)).GetTypes().Where(t => typeof(IAtsBehaviour).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var type in types)
            {
                behaviourList.Add((IAtsBehaviour)Activator.CreateInstance(type));
            }

            Behaviours = behaviourList.OrderBy(element => (Attribute.GetCustomAttribute(element.GetType(), typeof(AtsBehaviourPriority)) as AtsBehaviourPriority)?.Priority ?? LowestPriorty).ToArray();


            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.Awake(this);
            }
        }

        internal void OnDispose()
        {
            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.OnDestroy();
            }

            Behaviours = null;
        }

        internal void OnVehicleSpecPresented(AtsVehicleSpec vehicleSpec)
        {
            ControlHandle = new AtsControlHandle(vehicleSpec);

            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.OnActivate();
            }
        }

        internal void OnInitialize(int initialHandlePosition)
        {
            var convertedArgument = (AtsInitialHandlePosition)Enum.ToObject(typeof(AtsInitialHandlePosition), initialHandlePosition);

            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.OnStart(convertedArgument);
            }
        }

        internal void OnDoorStateChanged(bool isClosed)
        {
            if (isClosed)
            {
                CurrentStates.SetDoorState(AtsSimulationStates.DoorStateType.Close);
            }
            else
            {
                CurrentStates.SetDoorState(AtsSimulationStates.DoorStateType.Open);
            }
            
            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.OnDoorStateChanged(CurrentStates.DoorState);
            }
        }

        internal void OnKeyPress(int keyIndex)
        {
            var keyType = (AtsKey)Enum.ToObject(typeof(AtsKey), keyIndex);

            CurrentKeyStates[keyType].SetState(AtsKeyStates.KeyState.StateType.Down);
        }

        internal void OnKeyRelease(int keyIndex)
        {
            var keyType = (AtsKey)Enum.ToObject(typeof(AtsKey), keyIndex);

            CurrentKeyStates[keyType].SetState(AtsKeyStates.KeyState.StateType.Up);
        }

        internal void OnHornBlew(int hornIndex)
        {
            var convertedArgument = (AtsHornType)Enum.ToObject(typeof(AtsHornType), hornIndex);

            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.OnHornBlew(convertedArgument);
            }
        }

        internal void OnControlHandleMoved(int position, ControlHandleType controlHandleType)
        {
            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.OnControlHandleMoved(position, controlHandleType);
            }

            switch (controlHandleType)
            {
                case ControlHandleType.Brake:
                    ControlHandle.BrakePosition = position;
                    break;
                case ControlHandleType.Power:
                    ControlHandle.TractionPosition = position;
                    break;
                case ControlHandleType.Reverser:
                    ControlHandle.ReverserPosition = position;
                    break;
            }
        }
        
        internal void OnSignalChanged(int signalIndex)
        {
            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.OnSignalChanged(signalIndex);
            }
        }

        internal void OnBeaconDataRecieved(AtsBeaconData beaconData)
        {
            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.OnBeaconDataRecieved(beaconData);
            }
        }

        internal void OnUpdate(out AtsHandles vehicleOperations, AtsVehicleState vehicleState, IntPtr panelArray, IntPtr soundArray)
        {
            PanelOperations.SetSource(panelArray);
            SoundOperations.SetSource(soundArray);

            LastStates.CopyFrom(CurrentStates);
            CurrentStates.SetVehicleState(vehicleState);
            
            foreach (IAtsBehaviour behaviour in Behaviours)
            {
                behaviour.Update();
            }

            LastKeyStates.CopyFrom(CurrentKeyStates);

            vehicleOperations = ControlHandle.Handles;
        }
    }
}
