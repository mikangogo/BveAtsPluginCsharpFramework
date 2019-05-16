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
        public float YF { get => (float)Y; }


        public void Calculate(double deltaTime)
        {
            var nextY = LastY + deltaTime * LastU;        // おいらー法

            nextY = (IsAbsolute) && (0.0 > nextY) ? 0.0 : nextY;

            LastY = nextY;
            LastU = U;

            Y = nextY;
        }
	}
}
