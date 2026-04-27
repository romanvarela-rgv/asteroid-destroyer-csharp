using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EngineGDI
{
    static class Program
    {
        private static GameState currentState = GameState.Menu;
        public static float deltaTime;
        static DateTime lastFrameTime = DateTime.Now;
        public static bool showDebug = true;
        
        public static int SCREEN_WIDTH = 1024;
        public static int SCREEN_HEIGHT = 544;

        // Entidades del Juego
        public static Player p1;
        public static List<Asteroid> asteroids = new List<Asteroid>();
        public static List<Bullet> bullets = new List<Bullet>();
        private static Menu mainMenu;

        [STAThread]
        static void Main()
        {
            // 1. Inicialización del Motor
            Engine.Initialize(SCREEN_WIDTH, SCREEN_HEIGHT, "Asteroids GDI+");

            // 2. Inicialización de Componentes
            InitializeMenu();
            InitializeGame();

            // 3. Bucle Principal
            while (Engine.IsWindowOpen)
            {
                Engine.UpdateWindow();
                calcDeltatime();
                Update();
                Draw();

                Engine.Render();
            }
        }

        private static void Update()
        {

            switch (currentState)
            {
                case GameState.Menu:
                mainMenu.Update(deltaTime);
                    break;
                    
                case GameState.Playing:
                    UpdateGame(deltaTime);

                    if (GameManager.Instance.IsGameOver)
                        currentState = GameState.GameOver;

                    if (asteroids.Count == 0)
                        currentState = GameState.Victory;
                    break;

                case GameState.GameOver:
                case GameState.Victory:
                    if (Engine.OnKeyDown(Keys.R)) 
                    {
                        RestartGame();

                    }
                    break;
            }
            
        }

        private static void Draw()
        {
            switch(currentState)
            {
                case GameState.Menu:
                    mainMenu.Draw();
                    break;
                    
                case GameState.Playing:
                    DrawGame();
                    break;
                case GameState.Paused:
                    DrawGame();
                    Engine.DebugLog("--- PAUSED ---");
                    Engine.DebugLog("Press P to Resume");
                    break; 
                case GameState.GameOver:
                    DrawGame();
                    Engine.DebugLog("--- GAME OVER ---");
                    Engine.DebugLog("Press R to Try Again");
                    break;
                case GameState.Victory:
                    DrawGame();
                    Engine.DebugLog("--- VICTORY! ---");
                    Engine.DebugLog("Press R to Play Again");
                    break;
            }
           
        }

        private static void RestartGame()
        {
            GameManager.Instance.ResetGame();
            InitializeGame();
            currentState = GameState.Playing;
        }

        public static void InitializeMenu()
        {
            List<string> menuOptions = new List<string> { "INICIAR JUEGO", "SALIR" };
            mainMenu = new Menu(menuOptions, SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2 - 20);
            
            mainMenu.OnOptionSelected += (index) =>
            {
                if (index == 0)
                {
                    InitializeGame();
                    currentState = GameState.Playing;
                }
                else if (index == 1)
                {
                    Application.Exit();
                }
            };
        }

        public static void InitializeGame()
        {
            p1 = new Player("assets/textures/test.png", new Vector2f(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2));
            asteroids.Clear();
            bullets.Clear();

            Random rand = new Random();
            for (int i = 0; i < 8; i++)
            {
                asteroids.Add(new Asteroid(new Vector2f(rand.Next(0, SCREEN_WIDTH), rand.Next(0, SCREEN_HEIGHT))));
            }
        }

        public static void UpdateGame(float deltaTime)
        {
            if (Engine.OnKeyDown(Keys.Space))
            {
                bullets.Add(new Bullet(p1.Transform.Position, p1.Transform.Angle, p1.Velocity));
                Engine.PlaySound("assets/sounds/laser-zap-90575.wav");
            }

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
                    
                    // Ahora el tamaño se extrae automáticamente del Transform
                    if (Collision.CheckAABB(a.Transform, b.Transform))
                    {
                        
                        asteroids.RemoveAt(i);
                        bullets.RemoveAt(j);
                        Engine.PlaySound("assets/sounds/explosion-42132.wav");
                        break;
                    }
                }
            }
            foreach (var asteroid in asteroids)
            {
                if (Collision.CheckAABB(p1.Transform, asteroid.Transform))
                {
                    GameManager.Instance.IsGameOver = true;
                    Engine.PlaySound("assets/sounds/explosion-42132.wav");
                    break;
                }
            }
        }

        static void calcDeltatime()
        {
            TimeSpan deltaSpan = DateTime.Now - lastFrameTime;
            deltaTime = (float)deltaSpan.TotalSeconds;
            lastFrameTime = DateTime.Now;
        }
    }
}
