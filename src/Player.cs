using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EngineGDI
{
    public class Player : Character
    {
        // El sprite de la nave mira hacia arriba, pero en el juego angulo 0 = derecha
        private const float SPRITE_ANGLE_OFFSET = 90f;

        private const float INVULNERABLE_TIME = 2.5f;

        private Animation animation;
        private string lastFrame = "";
        private float invulnerableTimer;

        // Despues de reaparecer no lo pueden dañar por unos segundos
        public bool IsInvulnerable => invulnerableTimer > 0;

        public Player(Vector2f position) : base(position, 0.55f)
        {
            // Las turbinas titilan entre los 4 frames
            animation = new Animation(
                "idle",
                8f,
                new List<string>
                {
                    "assets/textures/Animations/Idle/0.png",
                    "assets/textures/Animations/Idle/1.png",
                    "assets/textures/Animations/Idle/2.png",
                    "assets/textures/Animations/Idle/3.png"
                },
                true
            );

            SetSprite(animation.CurrentFrame);

            Transform temp = Transform;
            temp.Origin = new Vector2f(0.5f, 0.5f);
            temp.Angle = -90f; // arranca apuntando hacia arriba
            Transform = temp;
        }

        // Vuelve al centro, quieto y apuntando hacia arriba, con invulnerabilidad
        public void Respawn(Vector2f position)
        {
            Transform temp = Transform;
            temp.Position = position;
            temp.Angle = -90f;
            Transform = temp;

            Velocity = new Vector2f(0, 0);
            invulnerableTimer = INVULNERABLE_TIME;
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

            animation.Update(deltaTime);
            string currentFrame = animation.CurrentFrame;
            if (currentFrame != lastFrame)
            {
                SetSprite(currentFrame);
                lastFrame = currentFrame;
            }

            if (invulnerableTimer > 0)
                invulnerableTimer -= deltaTime;
        }

        public override void Draw()
        {
            // Mientras es invulnerable parpadea (se dibuja un frame si y otro no, 10 veces por segundo)
            if (IsInvulnerable && (int)(invulnerableTimer * 10) % 2 == 0)
                return;

            Transform drawTransform = Transform;
            drawTransform.Angle += SPRITE_ANGLE_OFFSET;
            Engine.Draw(sprite, drawTransform);
        }
    }
}
