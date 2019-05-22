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

        public int OrderedTractionPosition
        {
            get
            {
                return Ordered.Power;
            }
        }

        public int OrderedBrakePosition
        {
            get
            {
                return Ordered.Brake;
            }
        }

        public int OrderedReverserPosition
        {
            get
            {
                return Ordered.Reverser;
            }
        }

        public int TractionPosition
        {
            get
            {
                return Operation.Power;
            }

            set
            {
                var changed = Operation;
                changed.Power = value;
                Operation = changed;
            }
        }

        public int BrakePosition
        {
            get
            {
                return Operation.Brake;
            }

            set
            {
                var changed = Operation;
                changed.Brake = value;
                Operation = changed;
            }
        }

        public int ReverserPosition
        {
            get
            {
                return Operation.Reverser;
            }

            set
            {
                var changed = Operation;
                changed.Reverser = value;
                Operation = changed;
            }
        }

        public int EmergencyBrake
        {
            get
            {
                return EmergencyServiceBrake + 1;
            }
        }

        public int EmergencyServiceBrake
        {
            get
            {
                return Spec.BrakeNotches;
            }
        }

        public int MaximumServiceBrake
        {
            get
            {
                return Spec.B67Notch;
            }
        }

        public int ServiceBrakeCanConfirmAts
        {
            get
            {
                return Spec.AtsNotch;
            }
        }

        public bool IsAppliedBrake
        {
            get
            {
                return (BrakePosition > 0);
            }
        }

        public bool IsAppliedServiceBrake
        {
            get
            {
                return (BrakePosition > 0) && (Operation.Brake <= EmergencyServiceBrake);
            }
        }

        public bool IsAppliedEmergencyServiceBrake
        {
            get
            {
                return (BrakePosition == EmergencyServiceBrake);
            }
        }

        public bool IsAppliedEmergencyBrake
        {
            get
            {
                return (BrakePosition == EmergencyBrake);
            }
        }

        public bool IsAppliedTraction
        {
            get
            {
                return (TractionPosition > 0);
            }
        }

        public bool IsPlacedOnReverserNeutral
        {
            get
            {
                return (ReverserPosition == 0);
            }
        }

        public bool IsPlacedOnReverserFront
        {
            get
            {
                return (ReverserPosition > 0);
            }
        }

        public bool IsPlacedOnReverserBack
        {
            get
            {
                return (ReverserPosition < 0);
            }
        }
    }
}
