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

        public static int SCREEN_WIDTH  = 1024;
        public static int SCREEN_HEIGHT = 544;

        // Entidades del juego
        public static Player           p1;
        public static List<Asteroid>   asteroids  = new List<Asteroid>();
        public static PoolObject<Bullet> bulletPool;

        // Escenas (interfaz)
        private static IScene mainMenuScene;
        private static IScene playMenuScene;
        private static IScene winScene;
        private static IScene loseScene;

        private static AsteroidFactory asteroidFactory = new AsteroidFactory();

        [STAThread]
        static void Main()
        {
            Engine.Initialize(SCREEN_WIDTH, SCREEN_HEIGHT, "Asteroids GDI+");

            InitializeMenu();
            InitializeGame();

            while (Engine.IsWindowOpen)
            {
                Engine.UpdateWindow();
                CalcDeltaTime();
                Update();
                Draw();
                Engine.Render();
            }
        }

      
        //  Maquina de estados — Update
      
        private static void Update()
        {
            switch (currentState)
            {
                case GameState.Menu:
                    mainMenuScene.Update(deltaTime);
                    break;

                case GameState.PlayMenu:
                    playMenuScene.Update(deltaTime);
                    break;

                case GameState.Playing:
                    if (Engine.OnKeyDown(Keys.P))
                        currentState = GameState.Paused;

                    UpdateGame(deltaTime);

                    if (GameManager.Instance.IsGameOver)
                        currentState = GameState.GameOver;

                    if (asteroids.Count == 0)
                        currentState = GameState.Victory;
                    break;

                case GameState.Paused:
                    if (Engine.OnKeyDown(Keys.P))
                        currentState = GameState.Playing;
                    break;

                case GameState.Victory:
                    winScene.Update(deltaTime);
                    break;

                case GameState.GameOver:
                    loseScene.Update(deltaTime);
                    break;
            }
        }

       
        //  Maquina de estados — Draw
       
        private static void Draw()
        {
            switch (currentState)
            {
                case GameState.Menu:
                    mainMenuScene.Draw();
                    break;

                case GameState.PlayMenu:
                    playMenuScene.Draw();
                    break;

                case GameState.Playing:
                    DrawGame();
                    break;

                case GameState.Paused:
                    DrawGame();
                    Engine.DebugLog("--- PAUSED ---");
                    Engine.DebugLog("Press P to Resume");
                    break;

                case GameState.Victory:
                    winScene.Draw();
                    break;

                case GameState.GameOver:
                    loseScene.Draw();
                    break;
            }
        }

        // Crea las escenas
        public static void InitializeMenu()
        {
            mainMenuScene = new MainMenuScene(
                onPlay:    () => currentState = GameState.PlayMenu,
                onOptions: () => { },
                onQuit:    () => Application.Exit()
            );

            playMenuScene = new PlayMenuScene(
                onNewGame:  () => { InitializeGame(); currentState = GameState.Playing; },
                onLoadGame: () => { InitializeGame(); currentState = GameState.Playing; },
                onBack:     () => currentState = GameState.Menu
            );

            winScene = new WinScene(
                onContinue: () => RestartGame(),
                onBack:     () => currentState = GameState.Menu
            );

            loseScene = new LoseScene(
                onRetry: () => RestartGame(),
                onBack:  () => currentState = GameState.Menu
            );
        }

        public static void InitializeGame()
        {
            p1         = new Player("assets/textures/test.png", new Vector2f(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2));
            asteroids.Clear();
            bulletPool = new PoolObject<Bullet>(100);

            for (int i = 0; i < 8; i++)
            {
                Asteroid a = asteroidFactory.CreateSafeAsteroid(p1.Transform.Position, SCREEN_WIDTH, SCREEN_HEIGHT);
                a.OnDestroyed += OnAsteroidExploded;
                asteroids.Add(a);
            }
        }

        private static void RestartGame()
        {
            GameManager.Instance.ResetGame();
            InitializeGame();
            currentState = GameState.Playing;
        }

        
        //  Logica de juego
        
        public static void UpdateGame(float dt)
        {
            if (Engine.OnKeyDown(Keys.Space))
            {
                bulletPool.GetObject().Init(p1.Transform.Position, p1.Transform.Angle, p1.Velocity);
                Engine.PlaySound("assets/sounds/laser-zap-90575.wav");
            }

            p1.Update(dt);

            foreach (var asteroid in asteroids)
                asteroid.Update(dt);

            for (int i = bulletPool.ActiveObjects.Count - 1; i >= 0; i--)
                bulletPool.ActiveObjects[i].Update(dt);

            CheckCollisions();

            if (showDebug)
            {
                Engine.DebugLog($"Asteroides: {asteroids.Count}");
                Engine.DebugLog($"Ship: {Math.Round(p1.Transform.Position.X)}, {Math.Round(p1.Transform.Position.Y)}");
            }
        }

        public static void DrawGame()
        {
            foreach (var asteroid in asteroids)
                asteroid.Draw();

            foreach (var bullet in bulletPool.ActiveObjects)
                bullet.Draw();

            p1.Draw();
        }

        private static void CheckCollisions()
        {
            var activeBullets = bulletPool.ActiveObjects;

            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                var asteroid = asteroids[i];

                for (int j = activeBullets.Count - 1; j >= 0; j--)
                {
                    var bullet = activeBullets[j];

                    if (Collision.CheckAABB(asteroid.Transform, bullet.Transform))
                    {
                        asteroid.Destroy();
                        asteroids.RemoveAt(i);
                        bullet.Deactivate();
                        bulletPool.RealeaseObject(bullet);

                        GameManager.Instance.Score++;

                        if (asteroids.Count == 0 && currentState == GameState.Playing)
                        {
                            currentState = GameState.Victory;
                            return;
                        }
                        break;
                    }
                }
            }

            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                if (Collision.CheckAABB(p1.Transform, asteroids[i].Transform))
                {
                    asteroids[i].Destroy();
                    asteroids.RemoveAt(i);
                    GameManager.Instance.IsGameOver = true;
                    break;
                }
            }
        }

        private static void OnAsteroidExploded(Asteroid asteroid)
        {
            Engine.PlaySound("assets/sounds/explosion-42132.wav");
        }

        static void CalcDeltaTime()
        {
            TimeSpan span = DateTime.Now - lastFrameTime;
            deltaTime    = (float)span.TotalSeconds;
            lastFrameTime = DateTime.Now;
        }
    }
}
