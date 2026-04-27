using System;

namespace EngineGDI
{
    public class Player : Character
    {
        public Player(string spritePath, float x, float y) : base(x, y, 0.4f)
        {
            SetSprite(spritePath);
            
            Transform temp = Transform;
            temp.Origin = new Vector2f(0.5f, 0.5f);
            Transform = temp;
        }

        public override void Update(float deltaTime)
        {
            float rotationSpeed = 200f;
            float acceleration = 300f;

            if (Engine.IsKeyDown(System.Windows.Forms.Keys.Left) || Engine.IsKeyDown(System.Windows.Forms.Keys.A))
            {
                Transform t = Transform;
                t.Angle -= rotationSpeed * deltaTime;
                Transform = t;
            }
            if (Engine.IsKeyDown(System.Windows.Forms.Keys.Right) || Engine.IsKeyDown(System.Windows.Forms.Keys.D))
            {
                Transform t = Transform;
                t.Angle += rotationSpeed * deltaTime;
                Transform = t;
            }

            if (Engine.IsKeyDown(System.Windows.Forms.Keys.Up) || Engine.IsKeyDown(System.Windows.Forms.Keys.W))
            {
                float angleRad = (float)(Transform.Angle * Math.PI / 180.0f);
                Vector2f dir = new Vector2f((float)Math.Cos(angleRad), (float)Math.Sin(angleRad));
                Velocity += dir * acceleration * deltaTime;
            }

            Velocity *= 0.99f; // Fricción
            base.Update(deltaTime);
        }

        public override void Draw()
        {
            Engine.Draw(sprite, Transform);
        }
    }
}
