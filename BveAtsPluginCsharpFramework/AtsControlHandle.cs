namespace AtsPlugin
{
    public class AtsControlHandle
    {
        public AtsHandles Handles { get; private set; }
        private AtsVehicleSpec Spec { get; set; }

        public AtsControlHandle(AtsVehicleSpec vehicleSpec)
        {
            Spec = vehicleSpec;
        }

        public int TractionPosition
        {
            get
            {
                return Handles.Power;
            }

            set
            {
                var changed = Handles;
                changed.Power = value;
                Handles = changed;
            }
        }

        public int BrakePosition
        {
            get
            {
                return Handles.Brake;
            }

            set
            {
                var changed = Handles;
                changed.Brake = value;
                Handles = changed;
            }
        }

        public int ReverserPosition
        {
            get
            {
                return Handles.Reverser;
            }

            set
            {
                var changed = Handles;
                changed.Reverser = value;
                Handles = changed;
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
                return (BrakePosition > 0) && (Handles.Brake <= EmergencyServiceBrake);
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
