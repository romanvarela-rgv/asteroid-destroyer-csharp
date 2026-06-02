using System;
using System.Drawing;

namespace EngineGDI
{
    public class ImageButton : IButton
    {
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

        public void Press()
        {
            OnPressed?.Invoke();
        }

        public void Draw()
        {
            Engine.Draw(spritePath, transform);

            if (isSelected)
            {
                Engine.DrawRectangle(
                    transform.Position,
                    transform.Size.X + 10,
                    transform.Size.Y + 10,
                    Color.Cyan,
                    false,      // outline, no relleno
                    0.5f, 0.5f
                );
            }
        }
    }
}
