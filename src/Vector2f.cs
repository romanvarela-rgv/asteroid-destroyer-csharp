using System;

namespace EngineGDI
{
    public struct Vector2f 
    {
        public float X;
        public float Y;

        public Vector2f(float x, float y)
        {
            X = x;
            Y = y;
        }

        // Operadores de suma
        public static Vector2f operator +(Vector2f a, Vector2f b) => new Vector2f(a.X + b.X, a.Y + b.Y);
        public static Vector2f operator +(Vector2f a, float b) => new Vector2f(a.X + b, a.Y + b);

        // Operadores de resta
        public static Vector2f operator -(Vector2f a, Vector2f b) => new Vector2f(a.X - b.X, a.Y - b.Y);
        public static Vector2f operator -(Vector2f a, float b) => new Vector2f(a.X - b, a.Y - b);

        // Operadores de multiplicación
        public static Vector2f operator *(Vector2f a, float b) => new Vector2f(a.X * b, a.Y * b);
        public static Vector2f operator *(float a, Vector2f b) => new Vector2f(a * b.X, a * b.Y);

        // Operadores de división
        public static Vector2f operator /(Vector2f a, float b) => new Vector2f(a.X / b, a.Y / b);

        // Métodos de utilidad
        public float Length() => (float)Math.Sqrt(X * X + Y * Y);

        public Vector2f Normalize()
        {
            float len = Length();
            return len > 0 ? this / len : new Vector2f(0, 0);
        }

        public static float Distance(Vector2f a, Vector2f b) => (a - b).Length();

        public static Vector2f Lerp(Vector2f a, Vector2f b, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return a + (b - a) * t;
        }

        public static float Lerp(float a, float b, float t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return a + (b - a) * t;
        }

        public override string ToString() => $"({X}, {Y})";
    }
}
