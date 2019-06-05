using System;


namespace AtsPlugin.Processing
{
    public class AtsOperationRationalDelay
    {
        private double LastY { get; set; } = 0.0;
        private AtsIntegrator Integrator { get; set; } = new AtsIntegrator();

        public double U { get; set; } = 0.0;
        public float UF { get => (float)U; set => U = value; }
        public double Y { get; private set; } = 0.0;
        public float YF { get => (float)Y; }
        public double Tp { get; set; } = 1000.0;
        public float TpF { get => (float)Tp; set => Tp = value; }
        public double DeltaY { get; set; } = 1.0;
        public float DeltaYF { get => (float)DeltaY; set => DeltaY = value;}


        public AtsOperationRationalDelay(double initial = 0.0)
        {
            Reset(initial);
        }

        public void Reset(double initial)
        {
            Integrator.Reset(initial, 0.0);
            LastY = initial;
        }

        public void Calculate(double deltaTime)
        {
            var delta = DeltaY * (deltaTime / Tp);
            var diff = U - Y;


            if (diff == 0.0)
            {
                return;
            }


            if (diff > 0.0)
            {
                Y += delta;


                if (Y >= U)
                {
                    Y = U;
                }
            }
            else if (diff < 0.0)
            {
                Y -= delta;


                if (Y <= U)
                {
                    Y = U;
                }
            }


            LastY = Y;
        }
    }
}