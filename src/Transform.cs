using System;

namespace EngineGDI
{
    public struct Transform
    {
        public Vector2f Position;
        public Vector2f Scale;
        public float Angle;

        public Transform(float x, float y, float scaleX = 1f, float scaleY = 1f, float angle = 0f)
        {
            this.Position = new Vector2f(x, y);
            this.Scale = new Vector2f(scaleX, scaleY);
            this.Angle = angle;
        }

        public Transform(Vector2f position, Vector2f scale, float angle = 0f)
        {
            this.Position = position;
            this.Scale = scale;
            this.Angle = angle;
        }
    }
}
