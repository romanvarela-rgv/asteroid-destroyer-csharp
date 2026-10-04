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
        public static bool showDebug = false; // F1 lo activa

        public static int SCREEN_WIDTH  = 1024;
        public static int SCREEN_HEIGHT = 544;

        // Entidades del juego
        public static Player           p1;
        public static List<Asteroid>   asteroids  = new List<Asteroid>();
        public static PoolObject<Bullet> bulletPool;
        public static List<Explosion>  explosions = new List<Explosion>();

        private static ScreenShake screenShake = new ScreenShake();

        // Oleadas: cada una trae mas asteroides grandes y mas rapidos. Al limpiar la ultima se gana.
        private const int MAX_WAVES = 3;
        private const float NEXT_WAVE_DELAY = 1.5f;
        private const float WAVE_BANNER_TIME = 2f;
        private static int currentWave;
        private static float nextWaveTimer;
        private static float waveBannerTimer;

        // Al perder una vida la nave reaparece despues de un momento
        private const float RESPAWN_DELAY = 1.2f;
        private static float respawnTimer;

        // Al ganar o perder se espera un poco antes de cambiar de pantalla para ver la explosion
        private const float END_DELAY = 1.2f;
        private static float endTimer;

        // Fondo de la partida (generado con tools/generate-ui-assets.ps1)
        private const string GAME_BG_PATH = "assets/textures/Scenes/GameBackground.png";
        private static Transform gameBackgroundTransform;

        private const string LIFE_ICON_PATH = "assets/textures/Animations/Idle/0.png";

        // Escenas — tipadas como IScene (interfaz)
        private static IScene mainMenuScene;
        private static IScene playMenuScene;
        private static IScene optionsScene;
        private static IScene winScene;
        private static IScene loseScene;
        private static PauseScene pauseScene; // tipo concreto para poder llamar a Open()

        private static AsteroidFactory asteroidFactory = new AsteroidFactory();

        private static Vector2f ScreenCenter => new Vector2f(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);

        // La nave esta en juego (no exploto ni esta esperando para reaparecer)
        private static bool PlayerActive => !GameManager.Instance.IsGameOver && respawnTimer <= 0;

        [STAThread]
        static void Main()
        {
            Engine.Initialize(SCREEN_WIDTH, SCREEN_HEIGHT, "Asteroids GDI+");

            // Record, opciones y partida guardada desde %AppData%\AsteroidDestroyer\save.txt
            GameManager.Instance.UseSaveStore(new FileSaveStore());
            Engine.SoundEnabled = GameManager.Instance.SoundEnabled;

            // La partida se crea recien al elegir New Game / Load Game
            // (crearla aca guardaria un checkpoint encima de la partida guardada)
            InitializeMenu();

            while (Engine.IsWindowOpen)
            {
                Engine.UpdateWindow();
                CalcDeltaTime();
                Update();
                Draw();
                Engine.Render();
            }

            // Por si se cierra en medio de una partida con un record nuevo
            GameManager.Instance.Save();
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

                case GameState.Options:
                    optionsScene.Update(deltaTime);
                    break;

                case GameState.Playing:
                    if (Engine.OnKeyDown(Keys.P) || Engine.OnKeyDown(Keys.Escape))
                    {
                        pauseScene.Open();
                        currentState = GameState.Paused;
                        break;
                    }

                    UpdateGame(deltaTime);
                    UpdateWaveProgress(deltaTime);
                    break;

                case GameState.Paused:
                    if (Engine.OnKeyDown(Keys.P) || Engine.OnKeyDown(Keys.Escape))
                        currentState = GameState.Playing;
                    else
                        pauseScene.Update(deltaTime);
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

                case GameState.Options:
                    optionsScene.Draw();
                    break;

                case GameState.Playing:
                    DrawGame();
                    break;

                case GameState.Paused:
                    DrawGame();
                    pauseScene.Draw();
                    break;

                case GameState.Victory:
                    winScene.Draw();
                    break;

                case GameState.GameOver:
                    loseScene.Draw();
                    break;
            }
        }


        //  Inicializacion


        // Crea las escenas y sus dependencias (lo que necesita para la transicion)
        public static void InitializeMenu()
        {
            var gm = GameManager.Instance;

            mainMenuScene = new MainMenuScene(
                onPlay:    () => currentState = GameState.PlayMenu,
                onOptions: () => currentState = GameState.Options,
                onQuit:    () => Application.Exit()
            );

            playMenuScene = new PlayMenuScene(
                onNewGame:  () => RestartGame(),
                onLoadGame: () => LoadGame(),
                onBack:     () => currentState = GameState.Menu
            );

            optionsScene = new OptionsScene(
                isSoundEnabled: () => gm.SoundEnabled,
                onToggleSound:  () => { gm.ToggleSound(); Engine.SoundEnabled = gm.SoundEnabled; },
                onBack:         () => currentState = GameState.Menu
            );

            pauseScene = new PauseScene(
                onResume:  () => currentState = GameState.Playing,
                onRestart: () => RestartGame(),
                onMenu:    () => currentState = GameState.Menu
            );

            Vector2f bgSize = Engine.GetTextureSize(GAME_BG_PATH);
            Transform bt = new Transform(0, 0);
            bt.Scale = new Vector2f(SCREEN_WIDTH / bgSize.X, SCREEN_HEIGHT / bgSize.Y);
            gameBackgroundTransform = bt;

            winScene = new WinScene(
                gameManager: gm,
                onContinue: () => RestartGame(),
                onBack:     () => currentState = GameState.Menu
            );

            loseScene = new LoseScene(
                gameManager: gm,
                onRetry: () => RestartGame(),
                onBack:  () => currentState = GameState.Menu
            );
        }

        public static void InitializeGame(int startWave)
        {
            p1         = new Player(ScreenCenter);
            asteroids.Clear();
            explosions.Clear();
            bulletPool = new PoolObject<Bullet>(100);
            screenShake.Reset();
            endTimer      = END_DELAY;
            respawnTimer  = 0;

            StartWave(startWave);
        }

        private static void RestartGame()
        {
            GameManager.Instance.ResetGame();
            InitializeGame(1);
            currentState = GameState.Playing;
        }

        // Sigue desde el inicio de la ultima oleada guardada (si no hay, arranca una nueva)
        private static void LoadGame()
        {
            var gm = GameManager.Instance;
            if (!gm.HasSavedGame)
            {
                RestartGame();
                return;
            }

            gm.LoadSavedGame();
            InitializeGame(gm.SavedWave);
            currentState = GameState.Playing;
        }


        //  Oleadas

        private static void StartWave(int wave)
        {
            currentWave     = wave;
            nextWaveTimer   = NEXT_WAVE_DELAY;
            waveBannerTimer = WAVE_BANNER_TIME;

            int count = 2 + wave;                     // 3, 4, 5 asteroides grandes
            float speed = 1f + 0.2f * (wave - 1);     // cada oleada un 20% mas rapida

            for (int i = 0; i < count; i++)
                AddAsteroid(asteroidFactory.CreateSafeAsteroid(p1.Transform.Position, SCREEN_WIDTH, SCREEN_HEIGHT,
                                                              AsteroidSize.Large, speed));

            GameManager.Instance.SaveCheckpoint(wave);
        }

        private static void UpdateWaveProgress(float dt)
        {
            var gm = GameManager.Instance;

            if (gm.IsGameOver)
            {
                endTimer -= dt;
                if (endTimer <= 0)
                    currentState = GameState.GameOver;
                return;
            }

            if (asteroids.Count > 0) return;

            if (currentWave < MAX_WAVES)
            {
                nextWaveTimer -= dt;
                if (nextWaveTimer <= 0)
                    StartWave(currentWave + 1);
            }
            else
            {
                endTimer -= dt;
                if (endTimer <= 0)
                {
                    gm.FinishGame();
                    currentState = GameState.Victory;
                }
            }
        }

        private static void AddAsteroid(Asteroid asteroid)
        {
            asteroid.OnDestroyed += OnAsteroidExploded;
            asteroids.Add(asteroid);
        }


        //  Logica de juego

        public static void UpdateGame(float dt)
        {
            if (Engine.OnKeyDown(Keys.F1))
                showDebug = !showDebug;

            if (respawnTimer > 0)
            {
                respawnTimer -= dt;
                if (respawnTimer <= 0)
                    p1.Respawn(ScreenCenter);
            }

            if (PlayerActive)
            {
                if (Engine.OnKeyDown(Keys.Space))
                {
                    bulletPool.GetObject().Init(p1.Transform.Position, p1.Transform.Angle, p1.Velocity);
                    Engine.PlaySound("assets/sounds/laser-zap-90575.wav");
                }

                p1.Update(dt);
            }

            foreach (var asteroid in asteroids)
                asteroid.Update(dt);

            UpdateBullets(dt);

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(dt);
                if (explosions[i].IsFinished)
                    explosions.RemoveAt(i);
            }

            screenShake.Update(dt);

            if (waveBannerTimer > 0)
                waveBannerTimer -= dt;

            CheckCollisions();

            if (showDebug)
            {
                Engine.DebugLog($"FPS {Math.Round(1f / Math.Max(dt, 0.0001f))}");
                Engine.DebugLog($"Wave {currentWave}  Asteroids {asteroids.Count}  Lives {GameManager.Instance.Lives}");
                Engine.DebugLog($"Ship: {Math.Round(p1.Transform.Position.X)}, {Math.Round(p1.Transform.Position.Y)}  angle {Math.Round(p1.Transform.Angle)}");
                Engine.DebugLog($"Bullets active {bulletPool.ActiveObjects.Count}  pooled {bulletPool.AvailableObjects.Count}");
            }
        }

        // Las balas que se vencen o salen de pantalla se desactivan solas:
        // aca se devuelven al pool para que no queden "invisibles" chocando asteroides
        private static void UpdateBullets(float dt)
        {
            var active = bulletPool.ActiveObjects;
            for (int i = active.Count - 1; i >= 0; i--)
            {
                active[i].Update(dt);
                if (!active[i].Active)
                    bulletPool.RealeaseObject(active[i]);
            }
        }

        public static void DrawGame()
        {
            // El fondo queda quieto; el shake mueve solo lo que esta en el espacio
            Engine.Draw(GAME_BG_PATH, gameBackgroundTransform);

            Engine.CameraOffset = screenShake.Offset;

            foreach (var asteroid in asteroids)
                asteroid.Draw();

            foreach (var bullet in bulletPool.ActiveObjects)
                bullet.Draw();

            if (PlayerActive)
                p1.Draw();

            foreach (var explosion in explosions)
                explosion.Draw();

            Engine.CameraOffset = new Vector2f(0, 0);

            DrawHud();
        }

        // HUD: puntaje y vidas arriba a la derecha, oleada abajo a la izquierda
        // y la tecla de pausa abajo a la derecha (arriba a la izquierda van los DebugLog con F1)
        private static void DrawHud()
        {
            var gm = GameManager.Instance;
            PixelText.Draw($"SCORE {gm.Score}", SCREEN_WIDTH - 20, 14, 3, Color.White, 1f);
            PixelText.Draw($"BEST {gm.HighScore}", SCREEN_WIDTH - 20, 44, 2, PixelText.Dim, 1f);
            DrawLives(gm.Lives);

            PixelText.Draw($"WAVE {currentWave}/{MAX_WAVES}", 20, SCREEN_HEIGHT - 32, 2, Color.White);
            PixelText.Draw("P  PAUSE", SCREEN_WIDTH - 20, SCREEN_HEIGHT - 32, 2, PixelText.Dim, 1f);

            // Cartel "WAVE N" al empezar cada oleada (en pausa no, para que no se pise con "PAUSED")
            if (waveBannerTimer > 0 && currentState == GameState.Playing)
            {
                string bannerPath = $"assets/textures/UI/Wave{currentWave}Title.png";
                Vector2f size = Engine.GetTextureSize(bannerPath);
                Transform bt = new Transform(SCREEN_WIDTH / 2f, SCREEN_HEIGHT / 2f - 90f);
                float s = 240f / size.X;
                bt.Scale  = new Vector2f(s, s);
                bt.Origin = new Vector2f(0.5f, 0.5f);
                Engine.Draw(bannerPath, bt);
            }
        }

        // Una navecita por cada vida, alineadas a la derecha debajo del record
        private static void DrawLives(int lives)
        {
            Vector2f iconSize = Engine.GetTextureSize(LIFE_ICON_PATH);
            float scale = 22f / iconSize.Y;
            float iconWidth = iconSize.X * scale;

            for (int i = 0; i < lives; i++)
            {
                Transform t = new Transform(SCREEN_WIDTH - 20 - i * (iconWidth + 6), 68f);
                t.Scale  = new Vector2f(scale, scale);
                t.Origin = new Vector2f(1f, 0f);
                Engine.Draw(LIFE_ICON_PATH, t);
            }
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
                        // Se saca de la lista antes del evento: los fragmentos se agregan al final
                        asteroids.RemoveAt(i);
                        asteroid.Destroy();
                        bullet.Deactivate();
                        bulletPool.RealeaseObject(bullet);

                        GameManager.Instance.AddScore(asteroid.Points);
                        break;
                    }
                }
            }

            if (!PlayerActive || p1.IsInvulnerable) return;

            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                var asteroid = asteroids[i];
                if (Collision.CheckAABB(p1.Transform, asteroid.Transform))
                {
                    asteroids.RemoveAt(i);
                    asteroid.Destroy();
                    OnPlayerHit();
                    break;
                }
            }
        }

        private static void OnPlayerHit()
        {
            // La nave explota mas grande y sacude mas fuerte
            explosions.Add(new Explosion(p1.Transform.Position, p1.Velocity, 140f));
            screenShake.Shake(0.5f, 14f);

            bool livesLeft = GameManager.Instance.LoseLife();
            if (livesLeft)
                respawnTimer = RESPAWN_DELAY;
        }

        private static void OnAsteroidExploded(Asteroid asteroid)
        {
            Engine.PlaySound("assets/sounds/explosion-42132.wav");

            // Explosion y sacudon proporcionales al tamaño del asteroide
            float diameter = Asteroid.GetDiameter(asteroid.Size);
            explosions.Add(new Explosion(asteroid.Transform.Position, asteroid.Velocity, diameter * 1.6f));
            screenShake.Shake(0.1f + diameter / 600f, diameter / 20f);

            // Se parte en dos asteroides mas chicos
            foreach (var fragment in asteroidFactory.CreateFragments(asteroid))
                AddAsteroid(fragment);
        }

        static void CalcDeltaTime()
        {
            TimeSpan span = DateTime.Now - lastFrameTime;
            deltaTime    = (float)span.TotalSeconds;
            lastFrameTime = DateTime.Now;
        }
    }
}
