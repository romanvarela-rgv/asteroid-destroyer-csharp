using System;

namespace EngineGDI
{
    public class Asteroid : Character
    {
        public event Action<Asteroid> OnDestroyed;

        private float rotationSpeed;

        public AsteroidSize Size { get; private set; }

        // Puntos que da al destruirlo: mas chico = mas dificil de pegarle = mas puntos
        public int Points => GetPoints(Size);

        public Asteroid(Vector2f position, Vector2f velocity, string spritePath, float angle, float rotationSpeed, AsteroidSize size)
            : base(position)
        {
            this.Size = size;
            this.Velocity = velocity;
            this.rotationSpeed = rotationSpeed;

            SetSprite(spritePath);

            // Los sprites miden 32, 64 o 128px: se escalan para que el tamaño en pantalla dependa solo de Size
            float scale = GetDiameter(size) / Transform.Size.X;

            Transform temp = Transform;
            temp.Angle  = angle;
            temp.Scale  = new Vector2f(scale, scale);
            temp.Origin = new Vector2f(0.5f, 0.5f);
            this.Transform = temp;
        }

        // Tamaño en pantalla, en pixeles
        public static float GetDiameter(AsteroidSize size)
        {
            switch (size)
            {
                case AsteroidSize.Large:  return 100f;
                case AsteroidSize.Medium: return 60f;
                default:                  return 34f;
            }
        }

        public static int GetPoints(AsteroidSize size)
        {
            switch (size)
            {
                case AsteroidSize.Large:  return 20;
                case AsteroidSize.Medium: return 50;
                default:                  return 100;
            }
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

        public void Destroy() //Aqui sirve como evento
        {
            OnDestroyed?.Invoke(this);
        }
    }
}
