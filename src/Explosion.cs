using System;
using System.Collections.Generic;

namespace EngineGDI
{
    public class Explosion : Character
    {
        private Animation animation;
        private string lastFrame = "";
        private float elapsed;
        private float startScale;
        private float endScale;

        public bool IsFinished => animation.IsFinished;

        // size: tamaño final aproximado en pixeles (los frames son de 128x128, ver tools/generate-ui-assets.ps1)
        public Explosion(Vector2f position, Vector2f velocity, float size) : base(position)
        {
            animation = new Animation(
                "explosion",
                18f,
                new List<string>
                {
                    "assets/textures/Explosion/1.png",
                    "assets/textures/Explosion/2.png",
                    "assets/textures/Explosion/3.png",
                    "assets/textures/Explosion/4.png",
                    "assets/textures/Explosion/5.png",
                    "assets/textures/Explosion/6.png",
                    "assets/textures/Explosion/7.png",
                    "assets/textures/Explosion/8.png"
                },
                false
            );

            SetSprite(animation.CurrentFrame);

            // Arranca chica y se expande hasta el tamaño pedido
            endScale = size / Transform.Size.X;
            startScale = endScale * 0.4f;

            Transform temp = Transform;
            temp.Origin = new Vector2f(0.5f, 0.5f);
            temp.Scale = new Vector2f(startScale, startScale);
            Transform = temp;

            // Hereda un poco del movimiento de lo que exploto
            Velocity = velocity * 0.3f;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            animation.Update(deltaTime);
            elapsed += deltaTime;

            string currentFrame = animation.CurrentFrame;
            if (currentFrame != lastFrame)
            {
                SetSprite(currentFrame);
                lastFrame = currentFrame;
            }

            // Ease-out: crece rapido al principio y despues frena
            float t = Math.Min(elapsed / animation.Duration, 1f);
            float eased = 1f - (1f - t) * (1f - t);
            float scale = startScale + (endScale - startScale) * eased;

            Transform temp = Transform;
            temp.Scale = new Vector2f(scale, scale);
            Transform = temp;
        }

        public override void Draw()
        {
            Camera.Draw(sprite, Transform);
        }
    }
}
