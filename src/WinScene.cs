using System;
using System.Drawing;

namespace EngineGDI
{
    public class WinScene : IScene
    {
        private const string BG_PATH = "assets/textures/Scenes/WinScene.png";
        private const string NEW_RECORD_PATH = "assets/textures/UI/NewRecordTitle.png";

      

        private Transform backgroundTransform;
        private ButtonGroup buttons = new ButtonGroup();
        private GameManager gameManager;

        public WinScene(GameManager gameManager, Action onContinue, Action onBack)
        {
            this.gameManager = gameManager;

            Vector2f bgSize = Engine.GetTextureSize(BG_PATH);
            Transform bt = new Transform(0, 0);
            bt.Scale = new Vector2f(Program.SCREEN_WIDTH / bgSize.X, Program.SCREEN_HEIGHT / bgSize.Y);
            backgroundTransform = bt;

            var btnContinue = new ImageButton("assets/textures/Buttons/ContinueButton.png", new Vector2f(512f, 330f), 130f);
            var btnBack     = new ImageButton("assets/textures/Buttons/BackButton.png",     new Vector2f(930f, 490f),  90f);

            btnContinue.OnPressed += onContinue;
            btnBack.OnPressed     += onBack;

            buttons.Add(btnContinue);
            buttons.Add(btnBack);
        }

        public void Update(float deltaTime)
        {
            buttons.Update();
        }

        public void Draw()
        {
            Engine.Draw(BG_PATH, backgroundTransform);
            DrawFinalScore();

            buttons.Draw();
        }

        // Puntaje centrado debajo del boton principal
        private void DrawFinalScore()
        {
            float cx = Program.SCREEN_WIDTH / 2f;
            PixelText.Draw($"SCORE {gameManager.Score}", cx, 388, 4, Color.White, 0.5f);
            PixelText.Draw($"BEST {gameManager.HighScore}", cx, 432, 2, PixelText.Dim, 0.5f);

            if (gameManager.IsNewRecord)
            {
                Vector2f size = Engine.GetTextureSize(NEW_RECORD_PATH);
                Transform t = new Transform(cx, 485);
                float s = 220f / size.X;
                t.Scale  = new Vector2f(s, s);
                t.Origin = new Vector2f(0.5f, 0.5f);
                Engine.Draw(NEW_RECORD_PATH, t);
            }
        }
    }
}
