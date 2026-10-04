using System;

namespace EngineGDI
{
    public class ImageButton : IButton
    {
        private const string SELECTOR_PATH = "assets/textures/UI/Selector.png";

        private Transform transform;
        private bool isSelected;
        private string spritePath;

        public Transform Transform { get => transform; set => transform = value; }
        public bool IsSelected   { get => isSelected; set => isSelected = value; }

        public event Action OnPressed;

        // targetWidth: ancho deseado en pixels de pantalla. 0 = tamaño nativo.
        public ImageButton(string spritePath, Vector2f position, float targetWidth = 0f)
        {
            this.spritePath = spritePath;

            Vector2f size = Engine.GetTextureSize(spritePath);
            Transform t = new Transform(position);
            t.Size   = size;
            t.Origin = new Vector2f(0.5f, 0.5f);

            if (targetWidth > 0f)
            {
                float s = targetWidth / size.X;
                t.Scale = new Vector2f(s, s);
            }

            transform = t;
        }

        // Cambia la imagen manteniendo la posicion y el ancho en pantalla (ej. Sound: On / Off)
        public void SetSprite(string path)
        {
            float width = transform.Size.X * transform.Scale.X;
            Vector2f size = Engine.GetTextureSize(path);

            spritePath = path;
            transform.Size = size;
            float s = width / size.X;
            transform.Scale = new Vector2f(s, s);
        }

        public void Press()
        {
            OnPressed?.Invoke();
        }

        public void Draw()
        {
            Engine.Draw(spritePath, transform);

            if (isSelected)
                DrawSelector();
        }

        // Flecha naranja a la izquierda del boton, con un pequeño vaiven
        private void DrawSelector()
        {
            // Tamaño del boton en pantalla (Size es el de la imagen sin escalar)
            float width  = transform.Size.X * transform.Scale.X;
            float height = transform.Size.Y * transform.Scale.Y;

            Vector2f selectorSize = Engine.GetTextureSize(SELECTOR_PATH);
            float scale = (height * 0.5f) / selectorSize.Y;

            float bob = (float)Math.Sin(Environment.TickCount / 120.0) * 3f;

            Transform st = new Transform(transform.Position.X - width / 2f - 2f + bob, transform.Position.Y);
            st.Scale  = new Vector2f(scale, scale);
            st.Origin = new Vector2f(1f, 0.5f);
            Engine.Draw(SELECTOR_PATH, st);
        }
    }
}
