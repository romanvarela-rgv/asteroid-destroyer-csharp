namespace EngineGDI
{
    public class GameManager
    {
        public const int START_LIVES = 3;

        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        private int score;
        private int lives;
        private bool isGameOver;
        private int highScoreAtStart; // para saber si esta partida batio el record

        private ISaveStore saveStore;
        private SaveData saveData = new SaveData();

        public int Score { get => score; }
        public int HighScore { get => saveData.HighScore; }
        public int Lives { get => lives; }
        public bool IsGameOver { get => isGameOver; }
        public bool IsNewRecord => score > 0 && score > highScoreAtStart;

        public bool SoundEnabled { get => saveData.SoundEnabled; }
        public bool HasSavedGame => saveData.HasCheckpoint;
        public int SavedWave => saveData.Wave;

        private GameManager()
        {
            ResetGame();
        }

        // Se llama una vez al arrancar: carga el record, las opciones y la partida guardada
        public void UseSaveStore(ISaveStore store)
        {
            saveStore = store;
            saveData = store.Load();
            highScoreAtStart = saveData.HighScore;
        }

        public void AddScore(int points)
        {
            score += points;
            if (score > saveData.HighScore)
                saveData.HighScore = score;
        }

        // Devuelve true si todavia quedan vidas
        public bool LoseLife()
        {
            if (isGameOver) return false;

            lives--;

            if (lives <= 0)
            {
                isGameOver = true;
                saveData.ClearCheckpoint(); // la partida termino, ya no hay nada que continuar
                Save();
            }
            return !isGameOver;
        }

        public void ResetGame()
        {
            score = 0;
            lives = START_LIVES;
            isGameOver = false;
            highScoreAtStart = saveData.HighScore;
        }

        // Retoma la partida guardada (puntaje y vidas del inicio de la ultima oleada)
        public void LoadSavedGame()
        {
            ResetGame();
            if (!saveData.HasCheckpoint) return;

            score = saveData.Score;
            lives = saveData.Lives;
        }

        // Al empezar cada oleada se guarda el progreso para poder seguir con "Load Game"
        public void SaveCheckpoint(int wave)
        {
            saveData.Wave = wave;
            saveData.Score = score;
            saveData.Lives = lives;
            Save();
        }

        // Al ganar: se guarda el record y se borra la partida en curso
        public void FinishGame()
        {
            saveData.ClearCheckpoint();
            Save();
        }

        public void ToggleSound()
        {
            saveData.SoundEnabled = !saveData.SoundEnabled;
            Save();
        }

        // Escribe el record y las opciones. El checkpoint solo cambia en SaveCheckpoint/FinishGame,
        // asi que guardar a mitad de oleada no adelanta la partida guardada.
        public void Save()
        {
            saveStore?.Save(saveData);
        }
    }
}
