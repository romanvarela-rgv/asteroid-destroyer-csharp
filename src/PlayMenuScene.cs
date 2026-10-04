using System;

namespace EngineGDI
{
    public class PlayMenuScene : IScene
    {
        private const string BG_PATH = "assets/textures/Scenes/PlayMenu.png";


        private Transform backgroundTransform;
        private ButtonGroup buttons = new ButtonGroup();

        public PlayMenuScene(Action onNewGame, Action onLoadGame, Action onBack)
        {
            Vector2f bgSize = Engine.GetTextureSize(BG_PATH);
            Transform bt = new Transform(0, 0);
            bt.Scale = new Vector2f(Program.SCREEN_WIDTH / bgSize.X, Program.SCREEN_HEIGHT / bgSize.Y);
            backgroundTransform = bt;

            float cx      = Program.SCREEN_WIDTH / 2f;   // 512
            float startY  = 290f;
            float spacing = 60f;

            var btnNew  = new ImageButton("assets/textures/Buttons/NewGameButton.png",  new Vector2f(cx, startY),           130f);
            var btnLoad = new ImageButton("assets/textures/Buttons/LoadGameButton.png", new Vector2f(cx, startY + spacing), 130f);
            var btnBack = new ImageButton("assets/textures/Buttons/BackButton.png",     new Vector2f(930f, 490f),            90f);

            btnNew.OnPressed  += onNewGame;
            btnLoad.OnPressed += onLoadGame;
            btnBack.OnPressed += onBack;

            buttons.Add(btnNew);
            buttons.Add(btnLoad);
            buttons.Add(btnBack);
        }

        // Seleccion y update de los botones
        public void Update(float deltaTime)
        {
            buttons.Update();
        }

        public void Draw()
        {
            Engine.Draw(BG_PATH, backgroundTransform);
            buttons.Draw();
        }
    }
}
