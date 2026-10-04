using System;

namespace EngineGDI
{
    public class MainMenuScene : IScene
    {
        private const string BG_PATH = "assets/textures/Scenes/MainMenu.png";

       

        private Transform backgroundTransform;
        private ButtonGroup buttons = new ButtonGroup();

        public MainMenuScene(Action onPlay, Action onOptions, Action onQuit)
        {
            Vector2f bgSize = Engine.GetTextureSize(BG_PATH);
            Transform bt = new Transform(0, 0);
            bt.Scale = new Vector2f(Program.SCREEN_WIDTH / bgSize.X, Program.SCREEN_HEIGHT / bgSize.Y);
            backgroundTransform = bt;

            float cx      = Program.SCREEN_WIDTH / 2f;   // 512
            float startY  = 290f;
            float spacing = 60f;

            var btnPlay    = new ImageButton("assets/textures/Buttons/PlayButton.png",    new Vector2f(cx, startY),              130f);
            var btnOptions = new ImageButton("assets/textures/Buttons/OptionsButton.png", new Vector2f(cx, startY + spacing),    130f);
            var btnQuit    = new ImageButton("assets/textures/Buttons/QuitButton.png",    new Vector2f(cx, startY + spacing * 2), 130f);

            btnPlay.OnPressed    += onPlay;
            btnOptions.OnPressed += onOptions;
            btnQuit.OnPressed    += onQuit;

            // añadimos los botones a la lista

            buttons.Add(btnPlay);
            buttons.Add(btnOptions);
            buttons.Add(btnQuit);
        }

        // seleccion de botones y update

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
