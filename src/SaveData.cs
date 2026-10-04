namespace EngineGDI
{
    // Lo que se guarda entre sesiones: el record, las opciones y el progreso de la partida
    public class SaveData
    {
        public int HighScore { get; set; }
        public bool SoundEnabled { get; set; } = true;

        // Checkpoint: oleada en la que se quedo la ultima partida (0 = no hay partida guardada)
        public int Wave { get; set; }
        public int Score { get; set; }
        public int Lives { get; set; }

        public bool HasCheckpoint => Wave > 0;

        public void ClearCheckpoint()
        {
            Wave = 0;
            Score = 0;
            Lives = 0;
        }
    }
}
