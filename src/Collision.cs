using System;

namespace EngineGDI
{
    public static class Collision
    {
       
        // Comprueba una colisión AABB entre dos transforms.
        // Utiliza automáticamente el campo Size del transform.
        
        public static bool CheckAABB(Transform t1, Transform t2)
        {
            // Calculamos las dimensiones reales aplicando la escala
            float w1 = t1.Size.X * t1.Scale.X;
            float h1 = t1.Size.Y * t1.Scale.Y;
            float w2 = t2.Size.X * t2.Scale.X;
            float h2 = t2.Size.Y * t2.Scale.Y;

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

        
        // Comprueba una colisión circular entre dos transforms.
        // Utiliza el tamaño (X o Y) para calcular un radio aproximado si no se especifica.
        
        public static bool CheckCircle(Transform t1, Transform t2)
        {
            float r1 = (t1.Size.X / 2f) * t1.Scale.X;
            float r2 = (t2.Size.X / 2f) * t2.Scale.X;
            float dist = Vector2f.Distance(t1.Position, t2.Position);
            return dist < (r1 + r2);
        }
    }
}
