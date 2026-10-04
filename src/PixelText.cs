using System.Collections.Generic;
using System.Drawing;

namespace EngineGDI
{
    // Dibuja texto con la fuente pixel del juego (la misma de los botones), con contorno negro
    // como los titulos originales. Cada fila del texto se arma con rectangulos.
    public static class PixelText
    {
        // Colores sacados del arte: gris claro del bisel de los botones y naranja de los titulos
        public static readonly Color Dim    = Color.FromArgb(0xB4, 0xB4, 0xB4);
        public static readonly Color Orange = Color.FromArgb(0xFF, 0x4D, 0x00);

        // Tramos horizontales de pixeles llenos, en unidades de la fuente
        private struct Run
        {
            public int X, Y, Length;
        }

        private class Layout
        {
            public List<Run> Runs = new List<Run>();
            public int Width;
        }

        // Los textos se repiten casi siempre ("SCORE 120"), asi que se calculan una sola vez
        private static Dictionary<string, Layout> cache = new Dictionary<string, Layout>();

        // pixel: tamaño en pantalla de cada pixel de la fuente (las mayusculas miden 7 pixeles de alto)
        // alignX: 0 = x es el borde izquierdo, 0.5 = centro, 1 = borde derecho
        public static void Draw(string text, float x, float y, int pixel, Color color, float alignX = 0f)
        {
            Layout layout = GetLayout(text);

            int left = (int)(x - layout.Width * pixel * alignX);
            int top  = (int)y;

            // Contorno: cada tramo agrandado un pixel para todos lados, en negro
            foreach (var run in layout.Runs)
                Engine.DrawRectangle(left + (run.X - 1) * pixel, top + (run.Y - 1) * pixel,
                                     (run.Length + 2) * pixel, 3 * pixel, Color.Black);

            foreach (var run in layout.Runs)
                Engine.DrawRectangle(left + run.X * pixel, top + run.Y * pixel,
                                     run.Length * pixel, pixel, color);
        }

        // Ancho en pantalla, por si hace falta para acomodar otras cosas
        public static int Width(string text, int pixel)
        {
            return GetLayout(text).Width * pixel;
        }

        private static Layout GetLayout(string text)
        {
            Layout layout;
            if (cache.TryGetValue(text, out layout))
                return layout;

            bool[,] mask = PixelFont.TextMask(text);
            layout = new Layout { Width = mask.GetLength(0) };

            for (int row = 0; row < mask.GetLength(1); row++)
            {
                int col = 0;
                while (col < mask.GetLength(0))
                {
                    if (!mask[col, row]) { col++; continue; }

                    int start = col;
                    while (col < mask.GetLength(0) && mask[col, row]) col++;
                    layout.Runs.Add(new Run { X = start, Y = row, Length = col - start });
                }
            }

            // El puntaje genera textos nuevos todo el tiempo: se limpia de vez en cuando
            if (cache.Count > 256)
                cache.Clear();

            cache[text] = layout;
            return layout;
        }
    }
}
