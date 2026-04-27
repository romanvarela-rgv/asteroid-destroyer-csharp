using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EngineGDI
{
    public class PlayingState : IGameState
    {
        private Player p1;
        private List<Asteroid> asteroids = new List<Asteroid>();
        private List<Bullet> bullets = new List<Bullet>();

        public void Initialize()
        {
            p1 = new Player("assets/textures/test.png", Program.SCREEN_WIDTH / 2, Program.SCREEN_HEIGHT / 2);

            Random rand = new Random();
            for (int i = 0; i < 8; i++)
            {
                asteroids.Add(new Asteroid(rand.Next(0, Program.SCREEN_WIDTH), rand.Next(0, Program.SCREEN_HEIGHT)));
            }
        }

        public void Update(float deltaTime)
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
            
            // Información de depuración
            if (Program.showDebug)
            {
                Engine.DebugLog($"Asteroides: {asteroids.Count}");
                Engine.DebugLog($"Disparos: {bullets.Count}");
                Engine.DebugLog($"Ship: {Math.Round(p1.Transform.Position.X)}, {Math.Round(p1.Transform.Position.Y)}");
            }
        }

        private void CheckCollisions()
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

        public void Draw()
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
    }
}
