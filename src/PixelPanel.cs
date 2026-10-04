using System.Drawing;

namespace EngineGDI
{
    // Panel con el mismo marco que los botones: contorno negro con esquinas recortadas,
    // cara gris, sombra a la izquierda/abajo y brillo arriba/derecha
    public static class PixelPanel
    {
        private static readonly Color Face   = Color.FromArgb(0x4D, 0x4D, 0x4D);
        private static readonly Color Shadow = Color.FromArgb(0x37, 0x37, 0x37);
        private static readonly Color Light  = Color.FromArgb(0x86, 0x86, 0x86);

        // b: grosor de cada borde. 4px es lo que mide el bisel de los botones en pantalla.
        public static void Draw(int x, int y, int width, int height, int b = 4)
        {
            // Contorno negro sin las esquinas
            Engine.DrawRectangle(x + b, y, width - 2 * b, height, Color.Black);
            Engine.DrawRectangle(x, y + b, width, height - 2 * b, Color.Black);

            int ix = x + b, iy = y + b, iw = width - 2 * b, ih = height - 2 * b;

            Engine.DrawRectangle(ix, iy, iw, ih, Face);
            Engine.DrawRectangle(ix + b, iy, iw - b, b, Light);           // arriba
            Engine.DrawRectangle(ix + iw - b, iy, b, ih, Light);          // derecha
            Engine.DrawRectangle(ix, iy + ih - b, iw - b, b, Shadow);     // abajo
            Engine.DrawRectangle(ix, iy, b, ih, Shadow);                  // izquierda
        }
    }
}
