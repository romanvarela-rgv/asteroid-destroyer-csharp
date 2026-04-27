using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace EngineGDI
{
    public static class Engine
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        private enum CommandType { Texture, Text, Rectangle }
        private class DrawCommand
        {
            public CommandType Type;
            public string TexturePath;
            public float X, Y, ScaleX, ScaleY;
            public float Angle, OffsetX, OffsetY;
            public string Text;
            public string FontPath;
            public Color Color;
            public int FontSize;
            public float Width, Height;
            public bool Fill;
        }
        private static Dictionary<string, Image> textures = new Dictionary<string, Image>();
        private static Dictionary<string, PrivateFontCollection> fontCollections = new Dictionary<string, PrivateFontCollection>();
        private static Dictionary<string, FontFamily> fontFamilies = new Dictionary<string, FontFamily>();
        private static List<DrawCommand> drawQueue = new List<DrawCommand>();
        private static GameForm window;
        public static bool IsWindowOpen { get; private set; } = false;
        public static Form Window => window;

        private static HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private static HashSet<Keys> handledKeys = new HashSet<Keys>();
        private static HashSet<Keys> releasedKeys = new HashSet<Keys>();
        private static HashSet<Keys> handledReleasedKeys = new HashSet<Keys>();

        private static List<string> debugMessages = new List<string>();
        private static Font debugFont = new Font("Consolas", 10);
        private static Brush debugBrush = Brushes.White;

        private static int masterVolume = 100; // 0 a 1000

        public static int MasterVolume
        {
            get { return masterVolume; }
            set { masterVolume = Math.Max(0, Math.Min(1000, value)); }
        }

        public static void Initialize(string title = "Game", int width = 800, int height = 600, bool fullscreen = false)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            window = new GameForm
            {
                Text = title,
                ClientSize = new Size(width, height),
                StartPosition = FormStartPosition.CenterScreen
            };
            if (fullscreen)
                window.WindowState = FormWindowState.Maximized;
            window.FormClosed += (s, e) => IsWindowOpen = false;
            window.KeyDown += (s, e) =>
            {
                if (!pressedKeys.Contains(e.KeyCode))
                {
                    pressedKeys.Add(e.KeyCode);
                    handledKeys.Remove(e.KeyCode);
                }

                releasedKeys.Remove(e.KeyCode);
                handledReleasedKeys.Remove(e.KeyCode);
            };
            window.KeyUp += (s, e) =>
            {
                pressedKeys.Remove(e.KeyCode);
                handledKeys.Remove(e.KeyCode);
                releasedKeys.Add(e.KeyCode);
                handledReleasedKeys.Remove(e.KeyCode);
            };
            window.Show();
            window.Focus();
            window.KeyPreview = true;
            IsWindowOpen = true;
        }

        public static void UpdateWindow()
        {
            if (window != null && window.Created)
                Application.DoEvents();
        }

        public static void PlaySound(string path)
        {
            // Usamos MCI para permitir sonidos superpuestos
            string alias = "Snd_" + Guid.NewGuid().ToString("N");
            
            // Abrir el archivo como mpegvideo (suele tener mejor soporte de volumen en MCI)
            mciSendString($"open \"{path}\" type mpegvideo alias {alias} wait", null, 0, IntPtr.Zero);
            
            // Intentar setear volumen (algunos drivers prefieren 'setaudio', otros 'set')
            // El rango suele ser 0-1000
            mciSendString($"setaudio {alias} volume to {masterVolume}", null, 0, IntPtr.Zero);
            mciSendString($"set {alias} volume to {masterVolume}", null, 0, IntPtr.Zero);
            
            // Reproducir y cerrar al terminar (el alias se libera)
            mciSendString($"play {alias} from 0", null, 0, IntPtr.Zero);
        }

        public static void Draw(string path, float x, float y, float scaleX = 1f, float scaleY = 1f, float angle = 0f, float offsetX = 0f, float offsetY = 0f)
        {
            if (!textures.ContainsKey(path))
                textures[path] = Image.FromFile(path);
            drawQueue.Add(new DrawCommand
            {
                Type = CommandType.Texture,
                TexturePath = path,
                X = x,
                Y = y,
                ScaleX = scaleX,
                ScaleY = scaleY,
                Angle = angle,
                OffsetX = offsetX,
                OffsetY = offsetY
            });
        }

        public static void Draw(string path, Transform transform)
        {
            Draw(path, transform.Position.X, transform.Position.Y, transform.Scale.X, transform.Scale.Y, transform.Angle, transform.Origin.X, transform.Origin.Y);
        }

        private static FontFamily GetFontFamily(string path)
        {
            if (string.IsNullOrEmpty(path) || path == "Arial") return FontFamily.GenericSansSerif;

            if (!fontFamilies.ContainsKey(path))
            {
                PrivateFontCollection pfc = new PrivateFontCollection();
                pfc.AddFontFile(path);
                fontCollections[path] = pfc;
                fontFamilies[path] = pfc.Families[0];
            }
            return fontFamilies[path];
        }

        public static float GetTextWidth(string text, int fontSize, string fontPath = null)
        {
            FontFamily family = GetFontFamily(fontPath);
            using (Font font = new Font(family, fontSize))
            using (Graphics g = window.CreateGraphics())
            {
                return g.MeasureString(text, font).Width;
            }
        }

        public static void DrawText(string text, float x, float y, Color color, int size = 12, float offsetX = 0f, float offsetY = 0f, string fontPath = null)
        {
            drawQueue.Add(new DrawCommand
            {
                Type = CommandType.Text,
                Text = text,
                FontPath = fontPath,
                X = x,
                Y = y,
                Color = color,
                FontSize = size,
                OffsetX = offsetX,
                OffsetY = offsetY
            });
        }

        public static void DrawText(string text, Vector2f position, Color color, int size = 12, float offsetX = 0f, float offsetY = 0f, string fontPath = null)
        {
            DrawText(text, position.X, position.Y, color, size, offsetX, offsetY, fontPath);
        }

        public static void DrawText(string text, Transform transform, Color color, int size = 12, string fontPath = null)
        {
            DrawText(text, transform.Position.X, transform.Position.Y, color, size, transform.Origin.X, transform.Origin.Y, fontPath);
        }

        public static void DrawRectangle(float x, float y, float width, float height, Color color, bool fill = true, float offsetX = 0f, float offsetY = 0f)
        {
            drawQueue.Add(new DrawCommand
            {
                Type = CommandType.Rectangle,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                Color = color,
                Fill = fill,
                OffsetX = offsetX,
                OffsetY = offsetY
            });
        }

        public static void DrawRectangle(Vector2f position, float width, float height, Color color, bool fill = true, float offsetX = 0f, float offsetY = 0f)
        {
            DrawRectangle(position.X, position.Y, width, height, color, fill, offsetX, offsetY);
        }

        public static void DrawRectangle(Transform transform, float width, float height, Color color, bool fill = true)
        {
            DrawRectangle(transform.Position.X, transform.Position.Y, width, height, color, fill, transform.Origin.X, transform.Origin.Y);
        }

        public static void Clear(Color color)
        {
            window.ClearColor = color;
        }

        public static bool OnKeyDown(Keys key)
        {
            if (pressedKeys.Contains(key) && !handledKeys.Contains(key))
            {
                handledKeys.Add(key);
                return true;
            }
            return false;
        }

        public static bool OnKeyUp(Keys key)
        {
            if (releasedKeys.Contains(key) && !handledReleasedKeys.Contains(key))
            {
                handledReleasedKeys.Add(key);
                return true;
            }
            return false;
        }

        public static bool IsKeyDown(Keys key)
        {
            return pressedKeys.Contains(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return OnKeyDown(key);
        }

        public static void DebugLog(string message)
        {
            debugMessages.Add(message);
        }

        public static void ClearDebug()
        {
            debugMessages.Clear();
        }

        private class GameForm : Form
        {
            public Color ClearColor = Color.Black;
            public GameForm()
            {
                DoubleBuffered = true;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.Clear(ClearColor);
                foreach (var cmd in drawQueue)
                {
                    if (cmd.Type == CommandType.Texture)
                    {
                        if (textures.ContainsKey(cmd.TexturePath))
                        {
                            var img = textures[cmd.TexturePath];
                            float width = img.Width * cmd.ScaleX;
                            float height = img.Height * cmd.ScaleY;
                            e.Graphics.TranslateTransform(cmd.X, cmd.Y);
                            e.Graphics.RotateTransform(cmd.Angle);
                            e.Graphics.DrawImage(
                                img,
                                -cmd.OffsetX * width,
                                -cmd.OffsetY * height,
                                width,
                                height
                            );
                            e.Graphics.ResetTransform();
                        }
                    }
                    else if (cmd.Type == CommandType.Text)
                    {
                        FontFamily family = GetFontFamily(cmd.FontPath);
                        using (Font font = new Font(family, cmd.FontSize))
                        using (Brush brush = new SolidBrush(cmd.Color))
                        {
                            SizeF size = e.Graphics.MeasureString(cmd.Text, font);
                            float tx = cmd.X - (size.Width * cmd.OffsetX);
                            float ty = cmd.Y - (size.Height * cmd.OffsetY);
                            e.Graphics.DrawString(cmd.Text, font, brush, tx, ty);
                        }
                    }
                    else if (cmd.Type == CommandType.Rectangle)
                    {
                        using (Brush brush = new SolidBrush(cmd.Color))
                        using (Pen pen = new Pen(cmd.Color))
                        {
                            float rx = cmd.X - (cmd.Width * cmd.OffsetX);
                            float ry = cmd.Y - (cmd.Height * cmd.OffsetY);
                            if (cmd.Fill)
                                e.Graphics.FillRectangle(brush, rx, ry, cmd.Width, cmd.Height);
                            else
                                e.Graphics.DrawRectangle(pen, rx, ry, cmd.Width, cmd.Height);
                        }
                    }
                }
                float debugY = 10;
                foreach (var msg in debugMessages)
                {
                    e.Graphics.DrawString(msg, debugFont, debugBrush, 10, debugY);
                    debugY += debugFont.Height + 2;
                }
                drawQueue.Clear();
            }
        }
    }
}
