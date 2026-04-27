using System;
using System.Drawing;

namespace EngineGDI
{
    public class Text
    {
        private string content;
        private Transform transform;
        private Color color;
        private int fontSize;

        public string Content { get => content; set => content = value; }
        public Transform Transform { get => transform; set => transform = value; }
        public Color Color { get => color; set => color = value; }
        public int FontSize { get => fontSize; set => fontSize = value; }

        public Text(string content, float x, float y, int fontSize = 24)
        {
            this.content = content;
            this.transform = new Transform(x, y);
            this.color = Color.White;
            this.fontSize = fontSize;
        }

        public void Draw()
        {
            Engine.DrawText(
                content, 
                transform.Position.X, 
                transform.Position.Y, 
                color, 
                fontSize, 
                transform.Origin.X, 
                transform.Origin.Y
            );
        }
    }
}
