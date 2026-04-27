using System;

namespace EngineGDI
{
    public class PlayingState : IGameState
    {
        public void Initialize()
        {
            // Delegamos la inicialización del juego a Program (abstracción Game)
            Program.InitializeGame();
        }

        public void Update(float deltaTime)
        {
            // Delegamos la actualización a Program
            Program.UpdateGame(deltaTime);
        }

        public void Draw()
        {
            // Delegamos el dibujo a Program
            Program.DrawGame();
        }
    }
}
