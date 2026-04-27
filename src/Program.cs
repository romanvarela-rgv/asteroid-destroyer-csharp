
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EngineGDI
{
    static class Program
    {
        public static float deltaTime;
        static DateTime lastFrameTime = DateTime.Now;

        public static bool showDebug = true;
        
        public static int SCREEN_WIDTH = 1024;
        public static int SCREEN_HEIGHT = 544;

        public static Player p1;
        public static List<Asteroid> asteroids = new List<Asteroid>();
        public static List<Bullet> bullets = new List<Bullet>();

        [STAThread]
        static void Main()
        {
            Engine.Initialize("ASTEROIDS DESTROYER", SCREEN_WIDTH, SCREEN_HEIGHT, false);

            // Inicializar Jugador
            p1 = new Player("assets/textures/test.png", SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);

            // Inicializar Asteroides
            Random rand = new Random();
            for (int i = 0; i < 8; i++)
            {
                asteroids.Add(new Asteroid(rand.Next(0, SCREEN_WIDTH), rand.Next(0, SCREEN_HEIGHT)));
            }

            while (Engine.IsWindowOpen)
            {
                Engine.UpdateWindow();
                calcDeltatime();

                Input();
                Update();
                Render();

                Engine.Clear(Color.Black);
                
                if (showDebug)
                {
                    Engine.ClearDebug();
                    Engine.DebugLog($"FPS: {Math.Round(1.0f / deltaTime)}");
                    Engine.DebugLog($"Asteroides: {asteroids.Count}");
                    Engine.DebugLog($"Disparos: {bullets.Count}");
                    Engine.DebugLog($"Ship: {Math.Round(p1.Transform.Position.X)}, {Math.Round(p1.Transform.Position.Y)}");
                }
                
                Engine.Window.Invalidate();
            }
        }

        static void calcDeltatime()
        {
            TimeSpan deltaSpan = DateTime.Now - lastFrameTime;
            deltaTime = (float)deltaSpan.TotalSeconds;
            if (deltaTime > 0.1f) deltaTime = 0.1f; 
            lastFrameTime = DateTime.Now;
        }

        static void Input()
        {
            // Disparar con Espacio
            if (Engine.OnKeyDown(Keys.Space))
            {
                bullets.Add(new Bullet(p1.Transform.Position.X, p1.Transform.Position.Y, p1.Transform.Angle, p1.Velocity));
                Engine.PlaySound("assets/sounds/laser-zap-90575.wav");
            }
        }

        static void Update()
        {
            p1.Update(deltaTime);

            // Actualizar Asteroides
            foreach (var asteroid in asteroids)
            {
                asteroid.Update(deltaTime);
            }

            // Actualizar Disparos y eliminar los viejos
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(deltaTime);
                if (bullets[i].LifeTime <= 0)
                {
                    bullets.RemoveAt(i);
                }
            }

            // Colisiones: Disparo vs Asteroide
            CheckCollisions();
        }

        static void CheckCollisions()
        {
            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                var a = asteroids[i];
                for (int j = bullets.Count - 1; j >= 0; j--)
                {
                    var b = bullets[j];
                    
                    // Distancia usando Vector2f
                    float dist = Vector2f.Distance(a.Transform.Position, b.Transform.Position);
                    
                    if (dist < 40 * a.Transform.Scale.X) // Radio aproximado del asteroide
                    {
                        asteroids.RemoveAt(i);
                        bullets.RemoveAt(j);
                        Engine.PlaySound("assets/sounds/explosion-42132.wav");
                        break;
                    }
                }
            }
        }

        static void Render()
        {
            foreach (var asteroid in asteroids)
            {
                asteroid.Draw();
            }

            foreach (var bullet in bullets)
            {
                bullet.Draw();
            }

            // Dibujar Jugador
            p1.Draw();
        }
    }
}
