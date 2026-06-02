using System;

namespace EngineGDI
{
    public interface IButton
    {
        Transform Transform { get; set; }
        bool IsSelected { get; set; }
        event Action OnPressed;
        void Press();
        void Draw();
    }
}
