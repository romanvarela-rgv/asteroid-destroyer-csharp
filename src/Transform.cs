namespace EngineGDI
{
    public struct Transform
    {
        public Vector2f Position;
        public Vector2f Scale;
        public float Angle;
        public Vector2f Origin;
        public Vector2f Size; // Tamaño base del objeto (sin escalar)

        public Transform(float x, float y, float scale = 1.0f)
        {
            Position = new Vector2f(x, y);
            Scale = new Vector2f(scale, scale);
            Angle = 0;
            Origin = new Vector2f(0, 0);
            Size = new Vector2f(0, 0);
        }

        public Transform(Vector2f position, float scale = 1.0f)
        {
            Position = position;
            Scale = new Vector2f(scale, scale);
            Angle = 0;
            Origin = new Vector2f(0, 0);
            Size = new Vector2f(0, 0);
        }
    }
}
