namespace AtsPlugin.Processing
{
    public class AtsIntegrator
    {
        private double LastU { get; set; } = 0.0;
        private double LastY { get; set; } = 0.0;

        public bool IsAbsolute { get; set; } = false;
        public double U { get; set; } = 0.0;
        public float UF { get => (float)U; set => U = value; }
        public double Y { get; private set; } = 0.0;
        public float YF => (float)Y;


        public AtsIntegrator(double initialY = 0.0, double initialLastU = 0.0)
        {
            Reset(initialY, initialLastU);
        }

        public void Reset(double initialY, double initialLastU)
        {
            LastY = initialY;
            LastU = initialLastU;
        }

        public void Calculate(double deltaTime)
        {
            var nextY = LastY + deltaTime * LastU;

            nextY = (IsAbsolute) && (0.0 > nextY) ? 0.0 : nextY;

            LastY = nextY;
            LastU = U;

            Y = nextY;
        }
	}
}
