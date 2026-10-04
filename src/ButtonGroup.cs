using System.Collections.Generic;
using System.Windows.Forms;

namespace EngineGDI
{
    // Lista de botones navegable con flechas/WASD y Enter/Espacio (la usan todas las escenas)
    public class ButtonGroup
    {
        private List<IButton> buttons = new List<IButton>();
        private int selectedIndex = 0;

        public void Add(IButton button)
        {
            buttons.Add(button);
            button.IsSelected = buttons.Count - 1 == selectedIndex;
        }

        // Vuelve a seleccionar el primer boton
        public void ResetSelection()
        {
            Select(0);
        }

        public void Update()
        {
            if (buttons.Count == 0) return;

            if (Engine.OnKeyDown(Keys.Up) || Engine.OnKeyDown(Keys.W))
                Select((selectedIndex - 1 + buttons.Count) % buttons.Count);

            if (Engine.OnKeyDown(Keys.Down) || Engine.OnKeyDown(Keys.S))
                Select((selectedIndex + 1) % buttons.Count);

            if (Engine.OnKeyDown(Keys.Enter) || Engine.OnKeyDown(Keys.Space))
                buttons[selectedIndex].Press();
        }

        public void Draw()
        {
            foreach (var btn in buttons)
                btn.Draw();
        }

        private void Select(int index)
        {
            if (buttons.Count == 0) return;

            buttons[selectedIndex].IsSelected = false;
            selectedIndex = index;
            buttons[selectedIndex].IsSelected = true;
        }
    }
}
