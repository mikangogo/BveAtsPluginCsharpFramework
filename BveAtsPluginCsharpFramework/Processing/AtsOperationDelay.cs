using System;


namespace AtsPlugin.Processing
{
    public class AtsOperationDelay
    {
        private double LastU { get; set; } = 0.0f;
        private AtsIntegrator Integrator { get; set; } = new AtsIntegrator();


        public bool IsAbsolute 
        {
            set
            {
                Integrator.IsAbsolute = value;
            }
            get
            {
                return Integrator.IsAbsolute;
            }
        }


        public double U { get; set; } = 0.0;
        public float UF
        {
            get
            {
                return (float)U;
            }
            set
            {
                U = value;
            }
        }

        public double Y { get; private set; } = 0.0f;
        public float YF
        {
            get
            {
                return (float)Y;
            }
        }

        public double Tp { get; set; } = 1000.0;
        public float TpF
        {
            get
            {
                return (float)Tp;
            }
            set
            {
                Tp = value;
            }
        }

        public AtsOperationDelay(double initial = 0.0)
        {
            Reset(initial);
        }

        public void Reset(double initial)
        {
            LastU = initial;
        }

        public void Calculate(double deltaTime)
        {
            Y = LastU = Integrator.Calculate((U - LastU) * (1.0 / Tp), deltaTime);
        }
    }
}
