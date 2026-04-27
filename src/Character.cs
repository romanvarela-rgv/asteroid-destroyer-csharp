using System;

namespace EngineGDI
{
    public abstract class Character
    {
        // Campos privados
        private Transform transform;
        private Vector2f velocity;
        protected string sprite;

        // Propiedades públicas
        public Transform Transform 
        { 
            get { return transform; } 
            set { transform = value; } 
        }

        public Vector2f Velocity 
        { 
            get { return velocity; } 
            set { velocity = value; } 
        }
        
        public Character(float x, float y, float scale = 1f)
        {
            this.transform = new Transform(x, y, scale);
        }
        
        protected void SetSprite(string path)
        {
            this.sprite = path;
            Transform t = this.Transform;
            t.Size = Engine.GetTextureSize(path);
            this.Transform = t;
        }

        public virtual void Update(float deltaTime)
        {
            transform.Position += velocity * deltaTime;
            Wrap();
        }

        protected virtual void Wrap()
        {
            if (transform.Position.X < -50) transform.Position.X = Program.SCREEN_WIDTH + 50;
            if (transform.Position.X > Program.SCREEN_WIDTH + 50) transform.Position.X = -50;
            if (transform.Position.Y < -50) transform.Position.Y = Program.SCREEN_HEIGHT + 50;
            if (transform.Position.Y > Program.SCREEN_HEIGHT + 50) transform.Position.Y = -50;
        }

        public abstract void Draw();
    }
}
