using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using EngineGDI;

public static class UiGen
{
    // Paleta sacada de los botones originales (PlayButton.png) y del titulo (MainMenu.png)
    static readonly Color Face    = Color.FromArgb(0x4D, 0x4D, 0x4D);
    static readonly Color Shadow  = Color.FromArgb(0x37, 0x37, 0x37);
    static readonly Color Light   = Color.FromArgb(0x86, 0x86, 0x86);
    static readonly Color Outline = Color.Black;
    static readonly Color Orange  = Color.FromArgb(0xFF, 0x4D, 0x00);

    static void FillCell(Bitmap bmp, int cx, int cy, int cell, Color c)
    {
        for (int y = cy * cell; y < (cy + 1) * cell; y++)
            for (int x = cx * cell; x < (cx + 1) * cell; x++)
                bmp.SetPixel(x, y, c);
    }

    static void Stamp(Bitmap bmp, bool[,] mask, int ox, int oy, int scale, Color c)
    {
        for (int y = 0; y < mask.GetLength(1); y++)
            for (int x = 0; x < mask.GetLength(0); x++)
                if (mask[x, y])
                    for (int py = 0; py < scale; py++)
                        for (int px = 0; px < scale; px++)
                            bmp.SetPixel(ox + x * scale + px, oy + y * scale + py, c);
    }

    // ---- Botones ----

    // Escala mas grande (hasta 12px por pixel de la fuente) con la que el texto entra en la cara del boton
    public static int FitScale(string text)
    {
        var mask = PixelFont.TextMask(text, true);
        return Math.Min(12, Math.Min(352 / mask.GetLength(0), 136 / mask.GetLength(1)));
    }

    // Boton 512x256 con el mismo marco que Play/Options/Quit (grilla de 32x16 celdas de 16px)
    public static void Button(string text, int scale, string outPath)
    {
        const int cell = 16;
        var bmp = new Bitmap(512, 256, PixelFormat.Format32bppArgb);

        for (int x = 3; x <= 28; x++) { FillCell(bmp, x, 1, cell, Outline); FillCell(bmp, x, 14, cell, Outline); }
        for (int y = 2; y <= 13; y++) { FillCell(bmp, 2, y, cell, Outline); FillCell(bmp, 29, y, cell, Outline); }
        for (int y = 2; y <= 13; y++)
            for (int x = 3; x <= 28; x++)
            {
                Color c = Face;
                if (x == 3) c = Shadow;              // borde izquierdo
                else if (y == 2) c = Light;          // borde superior
                else if (x == 28) c = Light;         // borde derecho
                else if (y == 13) c = Shadow;        // borde inferior
                FillCell(bmp, x, y, cell, c);
            }

        // Texto centrado en la cara del boton (celdas 4..27 x 3..12)
        var mask = PixelFont.TextMask(text, true);
        int ox = 256 - (mask.GetLength(0) * scale) / 2;
        int oy = 128 - (mask.GetLength(1) * scale) / 2;
        Stamp(bmp, mask, ox, oy, scale, Color.White);

        bmp.Save(outPath, ImageFormat.Png);
        bmp.Dispose();
    }

    // ---- Titulos ----

    // Estilo del titulo: relleno naranja, brillo blanco arriba de cada trazo y contorno negro
    static Bitmap TitleStyle(bool[,] mask, int scale)
    {
        int w = mask.GetLength(0) + 2, h = mask.GetLength(1) + 2;
        Func<int, int, bool> at = (x, y) =>
        {
            x -= 1; y -= 1;
            return x >= 0 && y >= 0 && x < mask.GetLength(0) && y < mask.GetLength(1) && mask[x, y];
        };

        var bmp = new Bitmap(w * scale, h * scale, PixelFormat.Format32bppArgb);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                Color? c = null;
                if (at(x, y))
                    c = at(x, y - 1) ? Orange : Color.White;
                else
                {
                    for (int dy = -1; dy <= 1 && c == null; dy++)
                        for (int dx = -1; dx <= 1; dx++)
                            if (at(x + dx, y + dy)) { c = Outline; break; }
                }
                if (c != null) FillCell(bmp, x, y, scale, c.Value);
            }
        return bmp;
    }

    public static void Title(string text, int scale, string outPath)
    {
        using (var bmp = TitleStyle(PixelFont.TextMask(text, true), scale))
            bmp.Save(outPath, ImageFormat.Png);
    }

    // Flecha que marca el boton seleccionado
    public static void Selector(int scale, string outPath)
    {
        const int w = 6, h = 11;
        var mask = new bool[w, h];
        for (int y = 0; y < h; y++)
        {
            int len = Math.Min(y, h - 1 - y) + 1;
            for (int x = 0; x < len && x < w; x++) mask[x, y] = true;
        }
        using (var bmp = TitleStyle(mask, scale))
            bmp.Save(outPath, ImageFormat.Png);
    }

    // ---- Fondo in-game ----

    static double Smooth(double[,] grid, double fx, double fy)
    {
        int gw = grid.GetLength(0), gh = grid.GetLength(1);
        int x0 = (int)fx, y0 = (int)fy;
        double tx = fx - x0, ty = fy - y0;
        tx = tx * tx * (3 - 2 * tx); ty = ty * ty * (3 - 2 * ty);
        Func<int, int, double> v = (x, y) => grid[Math.Min(x, gw - 1), Math.Min(y, gh - 1)];
        double a = v(x0, y0) + (v(x0 + 1, y0) - v(x0, y0)) * tx;
        double b = v(x0, y0 + 1) + (v(x0 + 1, y0 + 1) - v(x0, y0 + 1)) * tx;
        return a + (b - a) * ty;
    }

    static double Noise(int x, int y, double[][,] octaves, int[] cells)
    {
        double sum = 0, amp = 1, total = 0;
        for (int i = 0; i < octaves.Length; i++)
        {
            sum += Smooth(octaves[i], (double)x / cells[i], (double)y / cells[i]) * amp;
            total += amp; amp *= 0.5;
        }
        return sum / total;
    }

    static double[][,] Octaves(Random r, int w, int h, int[] cells)
    {
        var octaves = new double[cells.Length][,];
        for (int i = 0; i < cells.Length; i++)
        {
            octaves[i] = new double[w / cells[i] + 2, h / cells[i] + 2];
            for (int y = 0; y < octaves[i].GetLength(1); y++)
                for (int x = 0; x < octaves[i].GetLength(0); x++)
                    octaves[i][x, y] = r.NextDouble();
        }
        return octaves;
    }

    static Color Lerp(Color a, Color b, double t)
    {
        t = Math.Max(0, Math.Min(1, t));
        return Color.FromArgb(
            (int)(a.R + (b.R - a.R) * t),
            (int)(a.G + (b.G - a.G) * t),
            (int)(a.B + (b.B - a.B) * t));
    }

    // Espacio oscuro con nubes de polvo tenues, un planeta en la esquina y estrellas,
    // dibujado a 1/scale de resolucion y agrandado para que quede pixelado como las escenas
    public static void Background(int width, int height, int scale, int seed, string outPath)
    {
        var r = new Random(seed);
        int w = width / scale, h = height / scale;
        var small = new Bitmap(w, h, PixelFormat.Format32bppArgb);

        int[] cells = { 40, 20, 10 };
        var blueNoise = Octaves(r, w, h, cells);
        var brownNoise = Octaves(r, w, h, cells);

        // Bayer 4x4 para el dithering
        int[,] bayer = { { 0, 8, 2, 10 }, { 12, 4, 14, 6 }, { 3, 11, 1, 9 }, { 15, 7, 13, 5 } };

        Color space = Color.FromArgb(0x05, 0x06, 0x0D);
        Color dustBlue = Color.FromArgb(0x14, 0x19, 0x30);
        Color dustBrown = Color.FromArgb(0x2A, 0x19, 0x12);

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                double threshold = (bayer[y % 4, x % 4] + 0.5) / 16.0;
                double blue = (Noise(x, y, blueNoise, cells) - 0.5) * 3.0;    // nubes azuladas
                double brown = (Noise(x, y, brownNoise, cells) - 0.55) * 3.0; // polvo marron, mas escaso

                Color c = space;
                if (blue > threshold) c = dustBlue;
                if (brown > threshold) c = dustBrown;
                small.SetPixel(x, y, c);
            }

        // Planeta con bandas en la esquina inferior derecha (paleta del menu principal), oscurecido
        double pcx = w * 0.93, pcy = h * 1.05, pr = h * 0.42;
        Color[] bands =
        {
            Color.FromArgb(0x5A, 0x3A, 0x26), Color.FromArgb(0x3E, 0x52, 0x6E),
            Color.FromArgb(0x6E, 0x48, 0x2C), Color.FromArgb(0x4A, 0x2E, 0x20),
            Color.FromArgb(0x36, 0x48, 0x62)
        };
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                double dx = x - pcx, dy = y - pcy;
                double d = Math.Sqrt(dx * dx + dy * dy) / pr;
                if (d > 1) continue;

                double wobble = Math.Sin(x * 0.35 + y * 0.1) * 0.6;
                int band = (int)Math.Abs(Math.Floor((y + wobble) / 3.0)) % bands.Length;

                // Sombra hacia el borde superior izquierdo, con dithering
                double light = 1 - Math.Max(0, (-dx / pr) * 0.6 + (-dy / pr) * 0.4 + d * 0.5);
                double threshold = (bayer[y % 4, x % 4] + 0.5) / 16.0;
                double shade = light < threshold ? 0.25 : 0.55;
                if (d > 0.95) shade *= 0.6; // borde
                small.SetPixel(x, y, Lerp(Color.Black, bands[band], shade));
            }

        // Estrellas (mayoria grises, algunas blancas, azules o calidas)
        Color[] starColors =
        {
            Color.FromArgb(0x5A, 0x5A, 0x5A), Color.FromArgb(0x5A, 0x5A, 0x5A), Color.FromArgb(0x5A, 0x5A, 0x5A),
            Color.FromArgb(0x8C, 0x8C, 0x8C), Color.FromArgb(0x8C, 0x8C, 0x8C),
            Color.FromArgb(0xD0, 0xD0, 0xD0),
            Color.FromArgb(0x6F, 0x9F, 0xC8), Color.FromArgb(0xC8, 0x8A, 0x5A)
        };
        int stars = w * h / 90;
        for (int i = 0; i < stars; i++)
        {
            int x = r.Next(w), y = r.Next(h);
            double sdx = x - pcx, sdy = y - pcy;
            if (Math.Sqrt(sdx * sdx + sdy * sdy) < pr + 1.5) continue; // no sobre el planeta

            Color c = starColors[r.Next(starColors.Length)];
            small.SetPixel(x, y, c);

            // Algunas estrellas grandes con forma de cruz
            if (r.NextDouble() < 0.06 && x > 0 && y > 0 && x < w - 1 && y < h - 1)
            {
                Color dim = Lerp(Color.Black, c, 0.5);
                small.SetPixel(x - 1, y, dim); small.SetPixel(x + 1, y, dim);
                small.SetPixel(x, y - 1, dim); small.SetPixel(x, y + 1, dim);
                small.SetPixel(x, y, Color.White);
            }
        }

        using (var big = new Bitmap(width, height, PixelFormat.Format32bppArgb))
        {
            using (var g = Graphics.FromImage(big))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(small, 0, 0, width, height);
            }
            big.Save(outPath, ImageFormat.Png);
        }
        small.Dispose();
    }
}
