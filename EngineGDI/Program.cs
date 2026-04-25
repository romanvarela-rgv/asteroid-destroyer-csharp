
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;


namespace EngineGDI
{
    static class Program
    {
        public static bool showDebug = true;
        public static string currentMsg = "";

        public static int SCREEN_WIDTH = 1024;
        public static int SCREEN_HEIGHT = 780;

        [STAThread]
        static void Main()
        {
            Engine.Initialize("IERVA ENGINE", SCREEN_WIDTH, SCREEN_HEIGHT, false);

            //  Crear jugador
            Character player = new Character();
            player.transform = new Transform(200, 200);

            while (Engine.IsWindowOpen)
            {
                #region Engine Window Control
                Engine.UpdateWindow();
                #endregion

                //   LIMPIAR PANTALLA 
                Engine.Clear(Color.Black);

                //  INPUT
                player.Input();

                //  UPDATE
                player.Update();

                //  DRAW
                Engine.Draw("Background.png", 0, 0); // fondo
                player.Draw(); // jugador

                //  DEBUG
                if (showDebug)
                {
                    Engine.ClearDebug();
                    Engine.DebugLog(currentMsg);
                }

                //  FORZAR RENDER
                Engine.Window.Invalidate();
            }
        }
    }
}