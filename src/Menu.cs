using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EngineGDI
{
    public class Menu
    {
        private List<Text> menuItems = new List<Text>();
        private List<float> itemWidths = new List<float>();
        private int selectedIndex;
        private Transform transform;
        private Color normalColor = Color.White;
        private Color selectedColor = Color.Cyan;
        private int fontSize;
        private string fontPath;
        private float spacing;

        // Variables para la animación del selector
        private Vector2f selectorPos;
        private float selectorWidth;

        public event Action<int> OnOptionSelected;

        public Transform Transform 
        { 
            get { return transform; } 
            set 
            { 
                transform = value; 
                UpdateItemsPositions();
            } 
        }

        public Menu(List<string> options, float x, float y, int fontSize = 30, string fontPath = null)
        {
            this.transform = new Transform(x, y);
            this.fontSize = fontSize;
            this.fontPath = fontPath;
            this.selectedIndex = 0;
            this.spacing = fontSize * 1.8f;

            foreach (var opt in options)
            {
                Text item = new Text(opt, 0, 0);
                item.SetFont(fontPath, fontSize);
                                
                Transform t = item.Transform;
                t.Origin = new Vector2f(0.5f, 0.5f);
                item.Transform = t;

                menuItems.Add(item);
                itemWidths.Add(Engine.GetTextWidth(opt, fontSize, fontPath));
            }

            UpdateItemsPositions();

            // Inicializar posición del selector
            if (menuItems.Count > 0)
            {
                selectorPos = menuItems[0].Transform.Position;
                selectorWidth = itemWidths[0];
            }
        }

        private void UpdateItemsPositions()
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                Transform itemTransform = menuItems[i].Transform;
                itemTransform.Position = transform.Position + new Vector2f(0, i * spacing);
                menuItems[i].Transform = itemTransform;
            }
        }

        public void Update(float deltaTime)
        {
            if (Engine.OnKeyDown(Keys.Up) || Engine.OnKeyDown(Keys.W))
            {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = menuItems.Count - 1;
            }
            if (Engine.OnKeyDown(Keys.Down) || Engine.OnKeyDown(Keys.S))
            {
                selectedIndex++;
                if (selectedIndex >= menuItems.Count) selectedIndex = 0;
            }
            if (Engine.OnKeyDown(Keys.Enter) || Engine.OnKeyDown(Keys.Space))
            {
                OnOptionSelected?.Invoke(selectedIndex);
            }

            // Animación suave del selector usando Lerp
            if (menuItems.Count > 0)
            {
                Vector2f targetPos = menuItems[selectedIndex].Transform.Position;
                float targetWidth = itemWidths[selectedIndex] + 20;

                float speed = 15f;
                selectorPos = Vector2f.Lerp(selectorPos, targetPos, deltaTime * speed);
                selectorWidth = Vector2f.Lerp(selectorWidth, targetWidth, deltaTime * speed);
            }
        }

        public void Draw()
        {
            // Dibujar el selector animado
            Engine.DrawRectangle(
                selectorPos.X, 
                selectorPos.Y, 
                selectorWidth, 
                fontSize + 10, 
                Color.FromArgb(80, selectedColor), 
                true, 
                0.5f, 0.5f
            );

            for (int i = 0; i < menuItems.Count; i++)
            {
                bool isSelected = (i == selectedIndex);
                menuItems[i].Color = isSelected ? selectedColor : normalColor;
                
                // Dibujamos usando la clase Texto
                menuItems[i].Draw();
            }
        }
    }
}
