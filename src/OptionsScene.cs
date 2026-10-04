using System;
using System.Drawing;

namespace EngineGDI
{
    // Opciones: prender/apagar el sonido y ver los controles
    public class OptionsScene : IScene
    {
        private const string BG_PATH    = "assets/textures/Scenes/GameBackground.png";
        private const string TITLE_PATH = "assets/textures/UI/OptionsTitle.png";
        private const string SOUND_ON   = "assets/textures/Buttons/SoundOnButton.png";
        private const string SOUND_OFF  = "assets/textures/Buttons/SoundOffButton.png";

        private static readonly string[,] controls =
        {
            { "WASD / Arrows",    "Move" },
            { "Space",            "Shoot" },
            { "P / Esc",          "Pause" },
            { "Enter",            "Select" },
            { "F1",               "Debug info" }
        };

        private Transform backgroundTransform;
        private Transform titleTransform;
        private ButtonGroup buttons = new ButtonGroup();
        private ImageButton btnSound;
        private Func<bool> isSoundEnabled;

        public OptionsScene(Func<bool> isSoundEnabled, Action onToggleSound, Action onBack)
        {
            this.isSoundEnabled = isSoundEnabled;

            Vector2f bgSize = Engine.GetTextureSize(BG_PATH);
            Transform bt = new Transform(0, 0);
            bt.Scale = new Vector2f(Program.SCREEN_WIDTH / bgSize.X, Program.SCREEN_HEIGHT / bgSize.Y);
            backgroundTransform = bt;

            float cx = Program.SCREEN_WIDTH / 2f;

            Vector2f titleSize = Engine.GetTextureSize(TITLE_PATH);
            Transform tt = new Transform(cx, 90f);
            float titleScale = 340f / titleSize.X;
            tt.Scale  = new Vector2f(titleScale, titleScale);
            tt.Origin = new Vector2f(0.5f, 0.5f);
            titleTransform = tt;

            btnSound = new ImageButton(isSoundEnabled() ? SOUND_ON : SOUND_OFF, new Vector2f(cx, 190f), 150f);
            var btnBack = new ImageButton("assets/textures/Buttons/BackButton.png", new Vector2f(930f, 490f), 90f);

            btnSound.OnPressed += onToggleSound;
            btnSound.OnPressed += RefreshSoundButton; // despues de cambiar la opcion, actualiza la imagen
            btnBack.OnPressed  += onBack;

            buttons.Add(btnSound);
            buttons.Add(btnBack);
        }

        private void RefreshSoundButton()
        {
            btnSound.SetSprite(isSoundEnabled() ? SOUND_ON : SOUND_OFF);
        }

        public void Update(float deltaTime)
        {
            buttons.Update();
        }

        public void Draw()
        {
            Engine.Draw(BG_PATH, backgroundTransform);
            Engine.Draw(TITLE_PATH, titleTransform);

            // Panel con el marco de los botones
            PixelPanel.Draw(296, 244, 432, 212);
            PixelText.Draw("CONTROLS", Program.SCREEN_WIDTH / 2f, 264, 3, PixelText.Orange, 0.5f);
            for (int i = 0; i < controls.GetLength(0); i++)
            {
                float y = 304 + i * 28;
                PixelText.Draw(controls[i, 0], 495, y, 2, Color.White, 1f);
                PixelText.Draw(controls[i, 1], 530, y, 2, PixelText.Dim);
            }

            buttons.Draw();
        }
    }
}
