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

        public Bullet(Vector2f position, float angle, Vector2f shipVelocity) : base(position, 1.0f)
        {
            SetSprite("assets/textures/Animations/1.png");

            float speed = 400.0f;
            float angleRad = (float)(angle * Math.PI / 180.0f);
            
            this.Velocity = new Vector2f((float)Math.Cos(angleRad), (float)Math.Sin(angleRad)) * speed + shipVelocity;
            
            Transform temp = Transform;
            temp.Origin = new Vector2f(0.5f, 0.5f);
            this.Transform = temp;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            lifeTime -= deltaTime;
        }

        public override void Draw()
        {
            Engine.Draw(sprite, Transform);
        }
    }
}
