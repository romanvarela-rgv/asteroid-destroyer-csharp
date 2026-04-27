using System;

namespace EngineGDI
{
    public abstract class Character
    {
        // Campos privados (Ocultamiento de datos)
        private Transform transform;
        private Vector2f velocity;

        // Propiedades públicas (Encapsulamiento con get y set)
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
            this.transform = new Transform(x, y, scale, scale, 0);
        }

        public virtual void Update(float deltaTime)
        {
            // Usamos las propiedades internas
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
