using System;


namespace AtsPlugin.Processing
{
    public class AtsOperationDelay
    {
        private double LastU { get; set; } = 0.0;
        private AtsIntegrator Integrator { get; set; } = new AtsIntegrator();

        public bool IsAbsolute { get => Integrator.IsAbsolute; set => Integrator.IsAbsolute = value; }
        public double U { get; set; } = 0.0;
        public float UF { get => (float)U; set => U = value; }
        public double Y { get; private set; } = 0.0;
        public float YF => (float)Y;
        public double Tp { get; set; } = 1000.0;
        public float TpF { get => (float)Tp; set => Tp = value; }


        public AtsOperationDelay(double initial = 0.0)
        {
            Reset(initial);
        }

        public void Reset(double initial)
        {
            Integrator.Reset(initial, 0.0);
            LastU = initial;
        }

        public void Calculate(double deltaTime)
        {
            Integrator.U = (U - LastU) * (1.0 / Tp);

            Integrator.Calculate(deltaTime);

            Y = LastU = Integrator.Y;
        }
    }
}
