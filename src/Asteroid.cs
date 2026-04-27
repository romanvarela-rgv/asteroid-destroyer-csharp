using System;

namespace EngineGDI
{
    public class Asteroid : Character
    {
        private float rotationSpeed;
        private string sprite;
        private static Random random = new Random();

        public Asteroid(float x, float y) : base(x, y)
        {
            this.Velocity = new Vector2f(
                (float)(random.NextDouble() * 100 - 50),
                (float)(random.NextDouble() * 100 - 50)
            );

            Transform temp = Transform;
            temp.Angle = (float)(random.NextDouble() * 360);
            this.rotationSpeed = (float)(random.NextDouble() * 100 - 50);

            float scale = (float)(random.NextDouble() * 0.5 + 0.5);
            temp.Scale = new Vector2f(scale, scale);
            temp.Origin = new Vector2f(0.5f, 0.5f); // Centrar el asteroide
            this.Transform = temp;

            int spriteIndex = random.Next(1, 11);
            this.sprite = $"assets/textures/Animations/Idle_e/meteorito{spriteIndex}.png";
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
