using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EngineGDI
{
    static class Program
    {
        private static IGameState currentState;
        public static float deltaTime;
        static DateTime lastFrameTime = DateTime.Now;
        public static bool showDebug = true;
        
        public static int SCREEN_WIDTH = 1024;
        public static int SCREEN_HEIGHT = 544;

        [STAThread]
        static void Main()
        {
            Engine.Initialize("ASTEROIDS DESTROYER", SCREEN_WIDTH, SCREEN_HEIGHT, false);

            // Iniciamos con el estado de Menú
            ChangeState(new MenuState());

            while (Engine.IsWindowOpen)
            {
                Engine.UpdateWindow();
                calcDeltatime();

                // Delegamos la lógica al estado actual
                if (currentState != null)
                {
                    currentState.Update(deltaTime);
                    currentState.Draw();
                }

                Engine.Clear(Color.Black);
                
                if (showDebug)
                {
                    Engine.ClearDebug();
                    Engine.DebugLog($"FPS: {Math.Round(1.0f / deltaTime)}");
                    // Los estados pueden añadir sus propios logs
                }
                
                Engine.Window.Invalidate();
            }
        }

        public static void ChangeState(IGameState newState)
        {
            currentState = newState;
            currentState.Initialize();
        }

        static void calcDeltatime()
        {
            TimeSpan deltaSpan = DateTime.Now - lastFrameTime;
            deltaTime = (float)deltaSpan.TotalSeconds;
            if (deltaTime > 0.1f) deltaTime = 0.1f; 
            lastFrameTime = DateTime.Now;
        }
    }
}
