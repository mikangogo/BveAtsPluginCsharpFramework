namespace AtsPlugin
{
    public class AtsControlHandle
    {
        private AtsVehicleSpec Spec { get; set; }

        public AtsHandles Ordered { get; private set; }
        public AtsHandles Operation { get; private set; }
        

        public AtsControlHandle(AtsVehicleSpec vehicleSpec)
        {
            Spec = vehicleSpec;
        }

        public void Update()
        {
            Operation = Ordered;
        }

        public void SetOrderedHandlePosition(int position, AtsSimulationEnvironment.ControlHandleType controlHandleType)
        {
            var orderedHandles = Ordered;


            switch (controlHandleType)
            {
                case AtsSimulationEnvironment.ControlHandleType.Brake:
                    orderedHandles.Brake = position;
                    break;
                case AtsSimulationEnvironment.ControlHandleType.Power:
                    orderedHandles.Power = position;
                    break;
                case AtsSimulationEnvironment.ControlHandleType.Reverser:
                    orderedHandles.Reverser = position;
                    break;
            }


            Ordered = orderedHandles;
        }

        public int OrderedTractionPosition => Ordered.Power;
        public int OrderedBrakePosition => Ordered.Brake;
        public int OrderedReverserPosition => Ordered.Reverser;
        public int TractionPosition
        {
            get => Operation.Power;

            set
            {
                var changed = Operation;
                changed.Power = value;
                Operation = changed;
            }
        }
        public int BrakePosition
        {
            get => Operation.Brake;

            set
            {
                var changed = Operation;
                changed.Brake = value;
                Operation = changed;
            }
        }
        public int ReverserPosition
        {
            get => Operation.Reverser;

            set
            {
                var changed = Operation;
                changed.Reverser = value;
                Operation = changed;
            }
        }
        public int MaximumTractionPosition => Spec.PowerNotches;
        public int EmergencyBrake => EmergencyServiceBrake + 1;
        public int EmergencyServiceBrake => Spec.BrakeNotches;
        public int MaximumServiceBrake => Spec.B67Notch;
        public int ServiceBrakeCanConfirmAts => Spec.AtsNotch;
        public bool IsAppliedBrake => (BrakePosition > 0);
        public bool IsAppliedServiceBrake => (BrakePosition > 0) && (Operation.Brake <= EmergencyServiceBrake);
        public bool IsAppliedEmergencyServiceBrake => (BrakePosition == EmergencyServiceBrake);
        public bool IsAppliedEmergencyBrake => (BrakePosition == EmergencyBrake);
        public bool IsAppliedTraction => (TractionPosition > 0);
        public bool IsPlacedOnReverserNeutral => (ReverserPosition == 0);
        public bool IsPlacedOnReverserFront => (ReverserPosition > 0);
        public bool IsPlacedOnReverserBack => (ReverserPosition < 0);
    }
}
