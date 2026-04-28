using System;
using System.Collections.Generic;

namespace EngineGDI
{
    public class Bullet : Character
    {
        private float lifeTime = 2.0f;
        private Animation animation;
        private string lastFrame = "";

        public float LifeTime
        {
            get { return lifeTime; }
            set { lifeTime = value; }
        }

        public bool Active { get; private set; } = true;

        //  Constructor
        public Bullet(Vector2f position, float angle, Vector2f shipVelocity) : base(position, 1.0f)
        {

            animation = new Animation(
                "bullet",
                15f,
                new List<string>
                {
                    "assets/textures/Animations/1.png",
                    "assets/textures/Animations/2.png",
                    "assets/textures/Animations/3.png",
                    "assets/textures/Animations/4.png"
                },
                true
            );

            SetSprite(animation.CurrentFrame);

            //  Movimiento
            float speed = 400.0f;
            float angleRad = (float)(angle * Math.PI / 180.0f);

            this.Velocity = new Vector2f(
                (float)Math.Cos(angleRad),
                (float)Math.Sin(angleRad)
            ) * speed + shipVelocity;

            //  Centro del sprite
            Transform temp = Transform;
            temp.Origin = new Vector2f(0.5f, 0.5f);
            Transform = temp;
        }


        public void Init(Vector2f position, float angle, Vector2f shipVelocity)
        {
            this.lifeTime = 2.0f;
            this.Active = true;

            animation.Reset();
            SetSprite(animation.CurrentFrame);

            Transform temp = Transform;
            temp.Position = position;
            Transform = temp;

            float speed = 400.0f;
            float angleRad = (float)(angle * Math.PI / 180.0f);

            this.Velocity = new Vector2f(
                (float)Math.Cos(angleRad),
                (float)Math.Sin(angleRad)
            ) * speed + shipVelocity;
        }

        //  Update
        public override void Update(float deltaTime)
        {
            if (!Active) return;

            base.Update(deltaTime);

            animation.Update(deltaTime);
            string currentFrame = animation.CurrentFrame;

            if (currentFrame != lastFrame)
            {
                SetSprite(currentFrame);
                lastFrame = currentFrame;
            }

            // Lifetime
            lifeTime -= deltaTime;

            if (lifeTime <= 0 || IsOffScreen())
            {
                Deactivate();
            }
        }

        // Draw
        public override void Draw()
        {
            if (!Active) return;

            Engine.Draw(sprite, Transform);
        }

        // Desactivar (pooling)
        public void Deactivate()
        {
            Active = false;
        }

        // Check pantalla
        private bool IsOffScreen()
        {
            float margin = 50f;

            return Transform.Position.X < -margin ||
                   Transform.Position.X > Program.SCREEN_WIDTH + margin ||
                   Transform.Position.Y < -margin ||
                   Transform.Position.Y > Program.SCREEN_HEIGHT + margin;
        }
    }
}