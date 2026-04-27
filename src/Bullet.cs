using System;

namespace EngineGDI
{
    public class Bullet : Character
    {
        private float lifeTime = 2.0f;

        public float LifeTime
        {
            get { return lifeTime; }
            set { lifeTime = value; }
        }

        public Bullet(float x, float y, float angle, Vector2f shipVelocity) : base(x, y, 1.0f)
        {
            float speed = 400.0f;
            float angleRad = (float)(angle * Math.PI / 180.0f);
            
            this.Velocity = new Vector2f((float)Math.Cos(angleRad), (float)Math.Sin(angleRad)) * speed + shipVelocity;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            lifeTime -= deltaTime;
        }

        public override void Draw()
        {
            Engine.Draw("assets/textures/Animations/1.png", Transform.Position.X, Transform.Position.Y, 1.0f, 1.0f, 0, 0.5f, 0.5f);
        }
    }
}
