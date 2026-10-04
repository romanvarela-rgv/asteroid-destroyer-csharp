using System;
using System.Drawing;

namespace EngineGDI
{
    // Menu de pausa: se dibuja encima del juego congelado
    public class PauseScene : IScene
    {
        private const string TITLE_PATH = "assets/textures/UI/PausedTitle.png";

        private Transform titleTransform;
        private ButtonGroup buttons = new ButtonGroup();

        public PauseScene(Action onResume, Action onRestart, Action onMenu)
        {
            float cx = Program.SCREEN_WIDTH / 2f;

            Vector2f titleSize = Engine.GetTextureSize(TITLE_PATH);
            Transform tt = new Transform(cx, 150f);
            float titleScale = 300f / titleSize.X;
            tt.Scale  = new Vector2f(titleScale, titleScale);
            tt.Origin = new Vector2f(0.5f, 0.5f);
            titleTransform = tt;

            float startY  = 270f;
            float spacing = 65f;

            var btnResume  = new ImageButton("assets/textures/Buttons/ResumeButton.png",  new Vector2f(cx, startY),               130f);
            var btnRestart = new ImageButton("assets/textures/Buttons/RestartButton.png", new Vector2f(cx, startY + spacing),     130f);
            var btnMenu    = new ImageButton("assets/textures/Buttons/MenuButton.png",    new Vector2f(cx, startY + spacing * 2), 130f);

            btnResume.OnPressed  += onResume;
            btnRestart.OnPressed += onRestart;
            btnMenu.OnPressed    += onMenu;

            buttons.Add(btnResume);
            buttons.Add(btnRestart);
            buttons.Add(btnMenu);
        }

        // Al abrir la pausa siempre queda marcado "Resume"
        public void Open()
        {
            buttons.ResetSelection();
        }

        public void Update(float deltaTime)
        {
            buttons.Update();
        }

        public void Draw()
        {
            // Oscurece el juego que queda de fondo
            Engine.DrawRectangle(0, 0, Program.SCREEN_WIDTH, Program.SCREEN_HEIGHT, Color.FromArgb(170, 0, 0, 0));

            Engine.Draw(TITLE_PATH, titleTransform);
            buttons.Draw();

            PixelText.Draw("P / Esc to resume", Program.SCREEN_WIDTH / 2f, 468f, 2, PixelText.Dim, 0.5f);
        }
    }
}
