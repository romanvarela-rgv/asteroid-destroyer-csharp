using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EngineGDI
{
    public class LoseScene : IScene
    {
        private const string BG_PATH = "assets/textures/Scenes/LoseScene.png";

        

        private Transform backgroundTransform;
        private List<IButton> buttons = new List<IButton>();
        private int selectedIndex = 0;

        public LoseScene(Action onRetry, Action onBack)
        {
            Vector2f bgSize = Engine.GetTextureSize(BG_PATH);
            Transform bt = new Transform(0, 0);
            bt.Scale = new Vector2f(Program.SCREEN_WIDTH / bgSize.X, Program.SCREEN_HEIGHT / bgSize.Y);
            backgroundTransform = bt;

            var btnRetry = new ImageButton("assets/textures/Buttons/RetryButton.png", new Vector2f(512f, 315f), 130f);
            var btnBack  = new ImageButton("assets/textures/Buttons/BackButton.png",  new Vector2f(930f, 490f),  90f);

            btnRetry.OnPressed += onRetry;
            btnBack.OnPressed  += onBack;

            buttons.Add(btnRetry);
            buttons.Add(btnBack);

            buttons[selectedIndex].IsSelected = true;
        }

        public void Update(float deltaTime)
        {
            if (Engine.OnKeyDown(Keys.Up) || Engine.OnKeyDown(Keys.W))
            {
                buttons[selectedIndex].IsSelected = false;
                selectedIndex = (selectedIndex - 1 + buttons.Count) % buttons.Count;
                buttons[selectedIndex].IsSelected = true;
            }
            if (Engine.OnKeyDown(Keys.Down) || Engine.OnKeyDown(Keys.S))
            {
                buttons[selectedIndex].IsSelected = false;
                selectedIndex = (selectedIndex + 1) % buttons.Count;
                buttons[selectedIndex].IsSelected = true;
            }
            if (Engine.OnKeyDown(Keys.Enter) || Engine.OnKeyDown(Keys.Space))
            {
                buttons[selectedIndex].Press();
            }
        }

        public void Draw()
        {
            Engine.Draw(BG_PATH, backgroundTransform);
            foreach (var btn in buttons)
                btn.Draw();
        }
    }
}
