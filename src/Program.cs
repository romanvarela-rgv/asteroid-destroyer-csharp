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

        // Entidades del Juego (Program como abstracción de Game)
        public static Player p1;
        public static List<Asteroid> asteroids = new List<Asteroid>();
        public static List<Bullet> bullets = new List<Bullet>();

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

        // Lógica del Juego (Program como abstracción de Game)
        public static void InitializeGame()
        {
            p1 = new Player("assets/textures/test.png", SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
            asteroids.Clear();
            bullets.Clear();

            Random rand = new Random();
            for (int i = 0; i < 8; i++)
            {
                asteroids.Add(new Asteroid(rand.Next(0, SCREEN_WIDTH), rand.Next(0, SCREEN_HEIGHT)));
            }
        }

        public static void UpdateGame(float deltaTime)
        {
            // Entrada de disparo
            if (Engine.OnKeyDown(Keys.Space))
            {
                bullets.Add(new Bullet(p1.Transform.Position.X, p1.Transform.Position.Y, p1.Transform.Angle, p1.Velocity));
                Engine.PlaySound("assets/sounds/laser-zap-90575.wav");
            }

            // Actualización de entidades
            p1.Update(deltaTime);

            foreach (var asteroid in asteroids)
            {
                asteroid.Update(deltaTime);
            }

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(deltaTime);
                if (bullets[i].LifeTime <= 0)
                {
                    bullets.RemoveAt(i);
                }
            }

            CheckCollisions();
            
            if (showDebug)
            {
                Engine.DebugLog($"Asteroides: {asteroids.Count}");
                Engine.DebugLog($"Disparos: {bullets.Count}");
                Engine.DebugLog($"Ship: {Math.Round(p1.Transform.Position.X)}, {Math.Round(p1.Transform.Position.Y)}");
            }
        }

        public static void DrawGame()
        {
            foreach (var asteroid in asteroids)
            {
                asteroid.Draw();
            }

            foreach (var bullet in bullets)
            {
                bullet.Draw();
            }

            p1.Draw();
        }

        private static void CheckCollisions()
        {
            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                var a = asteroids[i];
                for (int j = bullets.Count - 1; j >= 0; j--)
                {
                    var b = bullets[j];
                    float dist = Vector2f.Distance(a.Transform.Position, b.Transform.Position);
                    
                    if (dist < 40 * a.Transform.Scale.X)
                    {
                        asteroids.RemoveAt(i);
                        bullets.RemoveAt(j);
                        Engine.PlaySound("assets/sounds/explosion-42132.wav");
                        break;
                    }
                }
            }
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
