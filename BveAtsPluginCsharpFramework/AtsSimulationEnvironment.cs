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


        private IAtsBehaviour[] BehaviourArray { get; set; } = null;

        public static AtsSimulationEnvironment Instance { get; private set; } = new AtsSimulationEnvironment();

        public readonly int LowestPriority = 1000000;
        public AtsBus Bus { get; private set; } = new AtsBus();
        public AtsNamedIoArray PanelOperations { get; private set; } = new AtsNamedIoArray();
        public AtsNamedIoArray SoundOperations { get; private set; } = new AtsNamedIoArray();
        public AtsControlHandle ControlHandle { get; private set; } = null;
        public AtsSimulationStates LastStates { get; private set; } = new AtsSimulationStates();
        public AtsSimulationStates CurrentStates { get; private set; } = new AtsSimulationStates();
        public AtsKeyStates LastKeyStates { get; private set; } = new AtsKeyStates();
        public AtsKeyStates CurrentKeyStates { get; private set; } = new AtsKeyStates();
        public double MaximumDeltaTime { get; set; } = 1000.0;
        public double DeltaTime => Math.Min(Math.Max(1.0, CurrentStates.SimulationTime - LastStates.SimulationTime), MaximumDeltaTime);
        public float DeltaTimeF => (float)DeltaTime;
        public bool WasJustInitialized { get; private set; } = false;
        public AtsPluginParameters PluginParameters { get; } = new AtsPluginParameters();


        internal static void CreateInstance()
        {
            Instance = new AtsSimulationEnvironment();
        }

        internal static void DisposeInstance()
        {
            Instance = null;
        }

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

            BehaviourArray = behaviourList.OrderBy(element => (Attribute.GetCustomAttribute(element.GetType(), typeof(AtsBehaviourPriority)) as AtsBehaviourPriority)?.Priority ?? LowestPriority).ToArray();


            foreach (var behaviour in BehaviourArray)
            {
                behaviour.Awake(this);
            }
        }

        internal void OnDispose()
        {
            foreach (var behaviour in BehaviourArray)
            {
                behaviour.OnDestroy();
            }

            BehaviourArray = null;
        }

        internal void OnVehicleSpecPresented(AtsVehicleSpec vehicleSpec)
        {
            ControlHandle = new AtsControlHandle(vehicleSpec);

            foreach (var behaviour in BehaviourArray)
            {
                behaviour.OnActivate();
            }
        }

        internal void OnInitialize(int initialHandlePosition)
        {
            WasJustInitialized = true;

            var convertedArgument = (AtsInitialHandlePosition)Enum.ToObject(typeof(AtsInitialHandlePosition), initialHandlePosition);

            foreach (var behaviour in BehaviourArray)
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
            
            foreach (var behaviour in BehaviourArray)
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

            foreach (var behaviour in BehaviourArray)
            {
                behaviour.OnHornBlew(convertedArgument);
            }
        }

        internal void OnControlHandleMoved(int position, ControlHandleType controlHandleType)
        {
            foreach (var behaviour in BehaviourArray)
            {
                behaviour.OnControlHandleMoved(position, controlHandleType);
            }


            ControlHandle.SetOrderedHandlePosition(position, controlHandleType);
        }
        
        internal void OnSignalChanged(int signalIndex)
        {
            foreach (var behaviour in BehaviourArray)
            {
                behaviour.OnSignalChanged(signalIndex);
            }
        }

        internal void OnBeaconDataReceived(AtsBeaconData beaconData)
        {
            foreach (var behaviour in BehaviourArray)
            {
                behaviour.OnBeaconDataReceived(beaconData);
            }
        }

        internal void OnUpdate(out AtsHandles vehicleOperations, AtsVehicleState vehicleState, IntPtr panelArray, IntPtr soundArray)
        {
            PanelOperations.SetSource(panelArray);
            SoundOperations.SetSource(soundArray);

            LastStates.CopyFrom(CurrentStates);
            if (WasJustInitialized)
            {
                LastStates.SetVehicleState(vehicleState);
            }

            CurrentStates.SetVehicleState(vehicleState);

            this.UpdateVelocityFromDeltaLocation();

            
            ControlHandle.Update();


            foreach (var behaviour in BehaviourArray)
            {
                behaviour.Update();
            }

            LastKeyStates.CopyFrom(CurrentKeyStates);

            vehicleOperations = ControlHandle.Operation;

            WasJustInitialized = false;
        }
    }
}
