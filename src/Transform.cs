using System;

namespace EngineGDI
{
    public struct Transform
    {
        public Vector2f Position;
        public Vector2f Scale;
        public Vector2f Origin; // 0.0 a 1.0 (0,0 es arriba-izq, 0.5,0.5 es centro)
        public float Angle;

        public Transform(float x, float y, float scaleX = 1f, float scaleY = 1f, float angle = 0f)
        {
            this.Position = new Vector2f(x, y);
            this.Scale = new Vector2f(scaleX, scaleY);
            this.Origin = new Vector2f(0, 0);
            this.Angle = angle;
        }

        public Transform(Vector2f position, Vector2f scale, float angle = 0f)
        {
            this.Position = position;
            this.Scale = scale;
            this.Origin = new Vector2f(0, 0);
            this.Angle = angle;
        }
    }
}
