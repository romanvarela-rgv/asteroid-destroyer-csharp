using System;
using System.Windows.Forms;

namespace EngineGDI
{
    public class Player : Character
    {
        public Player(string spritePath, Vector2f position) : base(position, 0.4f)
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

            if (Engine.IsKeyDown(Keys.Left) || Engine.IsKeyDown(Keys.A))
            {
                Transform t = Transform;
                t.Angle -= rotationSpeed * deltaTime;
                Transform = t;
            }
            if (Engine.IsKeyDown(Keys.Right) || Engine.IsKeyDown(Keys.D))
            {
                Transform t = Transform;
                t.Angle += rotationSpeed * deltaTime;
                Transform = t;
            }

            if (Engine.IsKeyDown(Keys.Up) || Engine.IsKeyDown(Keys.W))
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
