namespace AtsPlugin.Processing
{
    public class AtsIntegrator
    {
        private double LastU { get; set; } = 0.0f;
        private double LastY { get; set; } = 0.0f;

        public bool IsAbsolute { get; set; } = false;

        public float CalculateF(float u, float deltaTime)
        {
            return (float)Calculate(u, deltaTime);
        }

        public double Calculate(double u, double deltaTime)
        {
            double nextY = LastY + deltaTime * LastU;        // おいらー法

            nextY = (IsAbsolute) && (0.0f > nextY) ? 0.0f : nextY;

            LastY = nextY;
            LastU = u;

            return nextY;
        }
	}
}
