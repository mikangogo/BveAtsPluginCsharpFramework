using System;
using System.Runtime.InteropServices;

namespace AtsPlugin
{
    /// <summary>
    /// ATS Keys
    /// </summary>
    [Serializable]
    public enum AtsKey
    {
        S = 0,          // S Key
        A1,             // A1 Key
        A2,             // A2 Key
        B1,             // B1 Key
        B2,             // B2 Key
        C1,             // C1 Key
        C2,             // C2 Key
        D,              // D Key
        E,              // E Key
        F,              // F Key
        G,              // G Key
        H,              // H Key
        I,              // I Key
        J,              // J Key
        K,              // K Key
        L               // L Key
    }

    /// <summary>
    /// Initial Position of Handle
    /// </summary>
    [Serializable]
    public enum AtsInitialHandlePosition
    {
        ServiceBrake = 0,   // Service Brake
        EmergencyBrake,     // Emergency Brake
        Removed             // Handle Removed
    }

    /// <summary>
    /// Sound Control Instruction
    /// </summary>
    [Serializable]
    public static class AtsSoundControlInstruction
    {
        public const int Stop = -10000;     // Stop
        public const int Play = 1;          // Play Once
        public const int PlayLooping = 0;   // Play Repeatedly
        public const int Continue = 2;      // Continue
    }

    /// <summary>
    /// Type of Horn
    /// </summary>
    [Serializable]
    public enum AtsHornType
    {
        Primary = 0,    // Horn 1
        Secondary,      // Horn 2
        Music           // Music Horn
    }

    /// <summary>
    /// Constant Speed Control Instruction
    /// </summary>
    [Serializable]
    public static class AtsCscInstruction
    {
        public const int Continue = 0;       // Continue
        public const int Enable = 1;         // Enable
        public const int Disable = 2;        // Disable
    }

    /// <summary>
    /// Vehicle Specification
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct AtsVehicleSpec
    {
        public int BrakeNotches;   // Number of Brake Notches
        public int PowerNotches;   // Number of Power Notches
        public int AtsNotch;       // ATS Cancel Notch
        public int B67Notch;       // 80% Brake (67 degree)
        public int Cars;           // Number of Cars
    };

    /// <summary>
    /// State Quantity of Vehicle
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct AtsVehicleState
    {
        public double Location;    // Train Position (Z-axis) (m)
        public float Speed;        // Train Speed (km/h)
        public int Time;           // Time (ms)
        public float BcPressure;   // Pressure of Brake Cylinder (Pa)
        public float MrPressure;   // Pressure of MR (Pa)
        public float ErPressure;   // Pressure of ER (Pa)
        public float BpPressure;   // Pressure of BP (Pa)
        public float SapPressure;  // Pressure of SAP (Pa)
        public float Current;      // Current (A)
    };

    /// <summary>
    /// Received Data from Beacon
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct AtsBeaconData
    {
        public int Type;       // Type of Beacon
        public int Signal;     // Signal of Connected Section
        public float Distance; // Distance to Connected Section (m)
        public int Optional;   // Optional Data
    };

    /// <summary>
    /// Train Operation Instruction
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct AtsHandles
    {
        public int Brake;               // Brake Notch
        public int Power;               // Power Notch
        public int Reverser;            // Reverser Position
        public int ConstantSpeed;       // Constant Speed Control
    };

    /// <summary>
    /// Basics of ATS plug-in.
    /// </summary>
    public static class AtsCore
    {
        /// <summary>
        /// ATS Plug-in Version
        /// </summary>
        public const int Version = 0x00020000;

        /// <summary>
        /// Called when this plug-in is loaded
        /// </summary>
        [DllExport(CallingConvention.StdCall)]
        private static void Load()
        {
            AtsDebug.LogInfo($"Called: {nameof(Load)}()");


            AtsSimulationEnvironment.CreateInstance();
            AtsSimulationEnvironment.Instance.OnLoad();
        }

        /// <summary>
        /// Called when this plug-in is unloaded
        /// </summary>
        [DllExport(CallingConvention.StdCall)]
        private static void Dispose()
        {
            AtsDebug.LogInfo($"Called: {nameof(Dispose)}()");


            AtsSimulationEnvironment.Instance.OnDispose();
            AtsSimulationEnvironment.DisposeInstance();
        }

        /// <summary>
        /// Returns the version numbers of ATS plug-in
        /// </summary>
        /// <returns>Version numbers of ATS plug-in.</returns>
        [DllExport(CallingConvention.StdCall)]
        private static int GetPluginVersion()
        {
            return Version;
        }

        /// <summary>
        /// Called when the train is loaded
        /// </summary>
        /// <param name="vehicleSpec">Spesifications of vehicle.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void SetVehicleSpec(AtsVehicleSpec vehicleSpec)
        {
            AtsDebug.LogInfo($"Called: {nameof(SetVehicleSpec)}()");


            AtsSimulationEnvironment.Instance.OnVehicleSpecPresented(vehicleSpec);
        }

        /// <summary>
        /// Called when the game is started
        /// </summary>
        /// <param name="initialHandlePosition">Initial position of control handle.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void Initialize(int initialHandlePosition)
        {
            AtsDebug.LogInfo($"Called: {nameof(Initialize)}()");


            AtsSimulationEnvironment.Instance.OnInitialize(initialHandlePosition);
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        /// <param name="vehicleState">Current state of vehicle.</param>
        /// <param name="panel">Current state of panel.</param>
        /// <param name="sound">Current state of sound.</param>
        /// <returns>Driving operations of vehicle.</returns>
        [DllExport(CallingConvention.StdCall)]
        private static AtsHandles Elapse(AtsVehicleState vehicleState, IntPtr panel, IntPtr sound)
        {
            AtsSimulationEnvironment.Instance.OnUpdate(out var vehicleOperations, vehicleState, panel, sound);


            return vehicleOperations;
        }

        /// <summary>
        /// Called when the power is changed
        /// </summary>
        /// <param name="handlePosition">Position of traction control handle.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void SetPower(int handlePosition)
        {
            AtsDebug.LogInfo($"Called: {nameof(SetPower)}()");


            AtsSimulationEnvironment.Instance.OnControlHandleMoved(handlePosition, AtsSimulationEnvironment.ControlHandleType.Power);
        }

        /// <summary>
        /// Called when the brake is changed
        /// </summary>
        /// <param name="handlePosition">Position of brake control handle.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void SetBrake(int handlePosition)
        {
            AtsDebug.LogInfo($"Called: {nameof(SetBrake)}()");


            AtsSimulationEnvironment.Instance.OnControlHandleMoved(handlePosition, AtsSimulationEnvironment.ControlHandleType.Brake);
        }

        /// <summary>
        /// Called when the reverser is changed
        /// </summary>
        /// <param name="handlePosition">Position of reveerser handle.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void SetReverser(int handlePosition)
        {
            AtsDebug.LogInfo($"Called: {nameof(SetReverser)}()");


            AtsSimulationEnvironment.Instance.OnControlHandleMoved(handlePosition, AtsSimulationEnvironment.ControlHandleType.Reverser);
        }

        /// <summary>
        /// Called when any ATS key is pressed
        /// </summary>
        /// <param name="keyIndex">Index of key.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void KeyDown(int keyIndex)
        {
            AtsSimulationEnvironment.Instance.OnKeyPress(keyIndex);
        }

        /// <summary>
        /// Called when any ATS key is released
        /// </summary>
        /// <param name="keyIndex">Index of key.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void KeyUp(int keyIndex)
        {
            AtsSimulationEnvironment.Instance.OnKeyRelease(keyIndex);
        }

        /// <summary>
        /// Called when the horn is used
        /// </summary>
        /// <param name="hornIndex">Type of horn.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void HornBlow(int hornIndex)
        {
            AtsSimulationEnvironment.Instance.OnHornBlew(hornIndex);
        }

        /// <summary>
        /// Called when the door is opened
        /// </summary>
        [DllExport(CallingConvention.StdCall)]
        private static void DoorOpen()
        {
            AtsDebug.LogInfo($"Called: {nameof(DoorOpen)}()");


            AtsSimulationEnvironment.Instance.OnDoorStateChanged(false);
        }

        /// <summary>
        /// Called when the door is closed
        /// </summary>
        [DllExport(CallingConvention.StdCall)]
        private static void DoorClose()
        {
            AtsDebug.LogInfo($"Called: {nameof(DoorClose)}()");


            AtsSimulationEnvironment.Instance.OnDoorStateChanged(true);
        }

        /// <summary>
        /// Called when current signal is changed
        /// </summary>
        /// <param name="signalIndex">Index of signal.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void SetSignal(int signalIndex)
        {
            AtsDebug.LogInfo($"Called: {nameof(SetSignal)}()");


            AtsSimulationEnvironment.Instance.OnSignalChanged(signalIndex);
        }

        /// <summary>
        /// Called when the beacon data is received
        /// </summary>
        /// <param name="beaconData">Received data of beacon.</param>
        [DllExport(CallingConvention.StdCall)]
        private static void SetBeaconData(AtsBeaconData beaconData)
        {
            AtsDebug.LogInfo($"Called: {nameof(SetBeaconData)}()");


            AtsSimulationEnvironment.Instance.OnBeaconDataReceived(beaconData);
        }
    }
}
