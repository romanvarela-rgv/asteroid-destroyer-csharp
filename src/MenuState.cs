using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EngineGDI
{
    public class MenuState : IGameState
    {
        private Menu mainMenu;

        public void Initialize()
        {
            List<string> menuOptions = new List<string> { "INICIAR JUEGO", "SALIR" };
            mainMenu = new Menu(menuOptions, Program.SCREEN_WIDTH / 2 - 100, Program.SCREEN_HEIGHT / 2 - 50);
            
            mainMenu.OnOptionSelected += (index) =>
            {
                if (index == 0)
                {
                    Program.ChangeState(new PlayingState());
                }
                else if (index == 1)
                {
                    Application.Exit();
                }
            };
        }

        public void Update(float deltaTime)
        {
            mainMenu.Update();
        }

        public void Draw()
        {
            mainMenu.Draw();
        }
    }
}
