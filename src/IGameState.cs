using System;

namespace EngineGDI
{
    public interface IGameState
    {
        void Initialize();
        void Update(float deltaTime);
        void Draw();
    }
}
