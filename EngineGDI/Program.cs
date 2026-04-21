
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;


namespace EngineGDI
{
    static class Program
    {
        // Delta time
        public static float deltaTime;
        static DateTime lastFrameTime = DateTime.Now;

        // mostrar debug
        public static bool showDebug = true;
        public static string currentMsg = "";

        public static int SCREEN_WIDTH = 1024;
        public static int SCREEN_HEIGHT = 544;

        public static Player p1 = new Player("messi.png", 5, 10);
        private static float xMov = 0f;
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Engine.Initialize("IERVA ENGINE", SCREEN_WIDTH, SCREEN_HEIGHT, false);

            
         

            while (Engine.IsWindowOpen)
            {
                #region Engine Window Control
                Engine.UpdateWindow();
                #endregion

                calcDeltatime();

                Input();
                Update();
                Render();
         

                #region Engine Window Control
                Engine.Clear(Color.Black);
                currentMsg = deltaTime.ToString();
                // mensajes de debug
                if (showDebug)
                {
                    Engine.ClearDebug();
                    Engine.DebugLog(currentMsg);

                }
                Engine.Window.Invalidate();
                #endregion
            }
        }

        static void calcDeltatime()
        {
            TimeSpan deltaSpan = DateTime.Now - lastFrameTime;
            deltaTime = (float)deltaSpan.TotalSeconds;
            lastFrameTime = DateTime.Now;
        }


        static void Input()
        {
            if (Engine.OnKeyDown(Keys.Space))
            {
                p1.posX = 0;
            }
        }

        static void Update()
        {
            if (p1.posX > 1024)
            {
                p1.posX = 0;
            }

            p1.posX += 1;
        }

        static void Render()
        {
            Engine.Draw("cancha.png", 0, 0);
            Engine.Draw(p1.Sprite, p1.posX, SCREEN_HEIGHT / 2, 1, 1,0,.5f,.5f);
        }
       
    }
}
