using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EngineGDI
{
    public class Menu
    {
        private List<Text> menuItems = new List<Text>();
        private int selectedIndex;
        private Transform transform;
        private Color normalColor = Color.White;
        private Color selectedColor = Color.Cyan;
        private int fontSize;
        private float spacing;

        public event Action<int> OnOptionSelected;

        public Transform Transform 
        { 
            get { return transform; } 
            set 
            { 
                transform = value; 
                UpdateItemsPositions(); // Reposicionar si el transform del menú cambia
            } 
        }

        public Menu(List<string> options, float x, float y, int fontSize = 30)
        {
            this.transform = new Transform(x, y);
            this.fontSize = fontSize;
            this.selectedIndex = 0;
            this.spacing = fontSize * 1.8f;

            foreach (var opt in options)
            {
                Text item = new Text(opt, 0, 0, fontSize);
                
                // Al ser Transform un struct, hay que copiarlo, modificarlo y volverlo a asignar
                Transform t = item.Transform;
                t.Origin = new Vector2f(0.5f, 0.5f);
                item.Transform = t;

                menuItems.Add(item);
            }
            UpdateItemsPositions();
        }

        private void UpdateItemsPositions()
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                // Usamos transform y vec2d para calcular la posición de cada texto
                Transform itemTransform = menuItems[i].Transform;
                itemTransform.Position = transform.Position + new Vector2f(0, i * spacing);
                menuItems[i].Transform = itemTransform;
            }
        }

        public void Update()
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
        }

        public void Draw()
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                bool isSelected = (i == selectedIndex);
                menuItems[i].Color = isSelected ? selectedColor : normalColor;
                
                if (isSelected)
                {
                    // Dibujar el recuadro de fondo centrado usando el transform del texto
                    Engine.DrawRectangle(menuItems[i].Transform, 250, fontSize + 10, Color.FromArgb(50, selectedColor), true);
                }

                // Dibujamos usando la clase Texto
                menuItems[i].Draw();
            }
        }
    }
}
