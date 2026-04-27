using System;
using System.Windows.Forms;

namespace EngineGDI
{
    public class Player : Character
    {
        private string sprite;
        private float speed = 5.0f;
        private float rotationSpeed = 200.0f;
        private float friction = 0.98f;

        // Propiedad para el sprite
        public string Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        public Player(string sprite, float x, float y) : base(x, y, 0.4f)
        {
            this.sprite = sprite;
        }

        public override void Update(float deltaTime)
        {
            // Usamos las propiedades heredadas (get/set)
            Transform temp = Transform;

            if (Engine.IsKeyDown(Keys.Left)) temp.Angle -= rotationSpeed * deltaTime;
            if (Engine.IsKeyDown(Keys.Right)) temp.Angle += rotationSpeed * deltaTime;

            if (Engine.IsKeyDown(Keys.Up))
            {
                float angleRad = (float)(temp.Angle * Math.PI / 180.0f);
                Velocity += new Vector2f((float)Math.Cos(angleRad), (float)Math.Sin(angleRad)) * speed;
            }

            Transform = temp;

            base.Update(deltaTime);

            Velocity *= friction;
        }

        public override void Draw()
        {
            Engine.Draw(sprite, Transform.Position.X, Transform.Position.Y, Transform.Scale.X, Transform.Scale.Y, Transform.Angle, 0.5f, 0.5f);
        }

        protected override void Wrap()
        {
            Transform temp = Transform;
            if (temp.Position.X < 0) temp.Position.X = Program.SCREEN_WIDTH;
            if (temp.Position.X > Program.SCREEN_WIDTH) temp.Position.X = 0;
            if (temp.Position.Y < 0) temp.Position.Y = Program.SCREEN_HEIGHT;
            if (temp.Position.Y > Program.SCREEN_HEIGHT) temp.Position.Y = 0;
            Transform = temp;
        }
    }
}
