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
                // Calculamos el tamaño REAL en píxeles de pantalla multiplicando tamaño base por escala
                float realWidth = transform.Size.X * transform.Scale.X;
                float realHeight = transform.Size.Y * transform.Scale.Y;

                // Dibujamos el rectángulo de selección usando las medidas reales modificadas
                Engine.DrawRectangle(
                    transform.Position,
                    realWidth + 10,   // Le sumamos el pequeño margen de gracia de 10px que pusiste
                    realHeight + 10,
                    Color.Cyan,
                    false,            // Outline, sin relleno
                    0.5f, 0.5f        // Mismo pivote centrado para que encaje perfecto
                );
            }
        }
    }
}
