using System;

namespace EngineGDI
{
    public class Asteroid : Character
    {
        private float rotationSpeed;
        private static Random random = new Random();

        public Asteroid(Vector2f position, Vector2f velocity, string spritePath, float angle, float rotationSpeed, float scale ) 
            : base(position)
        {
            this.Velocity = new Vector2f(
                (float)(random.NextDouble() * 100 - 50),
                (float)(random.NextDouble() * 100 - 50)
            );

            SetSprite($"assets/textures/Animations/Idle_e/meteorito{random.Next(1, 11)}.png");

            Transform temp = Transform;
            temp.Angle = (float)(random.NextDouble() * 360);
            this.rotationSpeed = (float)(random.NextDouble() * 100 - 50);

            float Scale = (float)(random.NextDouble() * 0.5 + 0.5);
            temp.Scale = new Vector2f(scale, scale);
            temp.Origin = new Vector2f(0.5f, 0.5f);
            this.Transform = temp;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Transform temp = Transform;
            temp.Angle += rotationSpeed * deltaTime;
            this.Transform = temp;
        }

        public override void Draw()
        {
            Engine.Draw(sprite, Transform);
        }
    }
}
