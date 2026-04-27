using System;

namespace EngineGDI
{
    public static class Collision
    {
        /// <summary>
        /// Comprueba una colisión AABB (Axis-Aligned Bounding Box) entre dos transforms.
        /// </summary>
        /// <param name="t1">Transform del primer objeto</param>
        /// <param name="size1">Tamaño base del primer objeto (sin escalar)</param>
        /// <param name="t2">Transform del segundo objeto</param>
        /// <param name="size2">Tamaño base del segundo objeto (sin escalar)</param>
        /// <returns>True si hay colisión</returns>
        public static bool CheckAABB(Transform t1, Vector2f size1, Transform t2, Vector2f size2)
        {
            // Calculamos las dimensiones reales aplicando la escala
            float w1 = size1.X * t1.Scale.X;
            float h1 = size1.Y * t1.Scale.Y;
            float w2 = size2.X * t2.Scale.X;
            float h2 = size2.Y * t2.Scale.Y;

            // Calculamos los bordes teniendo en cuenta el origen (pivote)
            float left1 = t1.Position.X - (t1.Origin.X * w1);
            float right1 = left1 + w1;
            float top1 = t1.Position.Y - (t1.Origin.Y * h1);
            float bottom1 = top1 + h1;

            float left2 = t2.Position.X - (t2.Origin.X * w2);
            float right2 = left2 + w2;
            float top2 = t2.Position.Y - (t2.Origin.Y * h2);
            float bottom2 = top2 + h2;

            // Comprobación AABB estándar
            return right1 > left2 &&
                   left1 < right2 &&
                   bottom1 > top2 &&
                   top1 < bottom2;
        }

        /// <summary>
        /// Comprueba una colisión circular (por distancia) entre dos transforms.
        /// </summary>
        public static bool CheckCircle(Transform t1, float radius1, Transform t2, float radius2)
        {
            float dist = Vector2f.Distance(t1.Position, t2.Position);
            return dist < (radius1 * t1.Scale.X + radius2 * t2.Scale.X);
        }
    }
}
