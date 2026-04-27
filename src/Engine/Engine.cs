using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace EngineGDI
{
    public static class Engine
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        private class DrawCommand
        {
            public string TexturePath;
            public float X, Y, ScaleX, ScaleY;
            public float Angle, OffsetX, OffsetY;
        }
        private static Dictionary<string, Image> textures = new Dictionary<string, Image>();
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
