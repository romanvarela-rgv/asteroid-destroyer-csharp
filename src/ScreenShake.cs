using System;

namespace EngineGDI
{
    public class ScreenShake
    {
        private static Random random = new Random();

        private float duration;
        private float timeLeft;
        private float intensity;

        public Vector2f Offset { get; private set; }

        // intensity: desplazamiento maximo en pixeles. Si ya hay un shake mas fuerte, no lo pisa.
        public void Shake(float duration, float intensity)
        {
            if (timeLeft > 0 && intensity < CurrentIntensity()) return;

            this.duration = duration;
            this.timeLeft = duration;
            this.intensity = intensity;
        }

        public void Update(float deltaTime)
        {
            if (timeLeft <= 0)
            {
                Offset = new Vector2f(0, 0);
                return;
            }

            timeLeft -= deltaTime;

            float current = CurrentIntensity();
            Offset = new Vector2f(
                (float)(random.NextDouble() * 2 - 1) * current,
                (float)(random.NextDouble() * 2 - 1) * current
            );
        }

        public void Reset()
        {
            timeLeft = 0;
            Offset = new Vector2f(0, 0);
        }

        // La intensidad baja linealmente hasta 0
        private float CurrentIntensity()
        {
            return duration > 0 ? intensity * Math.Max(timeLeft / duration, 0f) : 0f;
        }
    }
}
