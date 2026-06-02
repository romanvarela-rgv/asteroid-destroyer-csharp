using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EngineGDI
{
    public class ImageMenu
    {
        /*
        private List<Text> normalSprites = new List<string>();
        private List<float> itemWidths = new List<string>();

        private List<Vector2f> itemSizes = new List<Vector2f>();
        private int selectedIndex;
        private Transform transform;
        private float spacing;

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

        public ImageMenu(List<string> normalPaths, List<string> selectedPaths, float x, float y, float spacing = 80f)
        {
            this.transform = new Transform(x, y);
            this.normalSprites = normalPaths;
            this.selectedSprites = selectedPaths;
            this.selectedIndex = 0;
            this.spacing = spacing;

            foreach (var path in normalSprites)
            {
                itemSizes.Add(Engine.GetTextureSize(path));
            }

        }
       private void Update(float deltaTime)
        {
            if (Engine.OnKeyDown(Keys.Up) || Engine.OnKeyDown(Keys.W))
            {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = normalSprites.Count - 1;
            } 
            if(Engine.OnKeyDown(Keys.Down) || Engine.OnKeyDown(Keys.S))
            {
                selectedIndex++;
                if (selectedIndex >= normalSprites.Count) selectedIndex = 0;
            }
            if (Engine.OnKeyDown(Keys.Enter) || Engine.OnKeyDown(Keys.Space))
            {
                OnOptionSelected?.Invoke(selectedIndex);
            }
        }

        public void Draw()
        {
            for (int i = 0; i < normalSprites.Count; i++)
            {
                bool isSelected = (i == selectedIndex);
                string spriteToDraw = isSelected ? selectedSprites[i] : normalSprites[i];

                Transform itemTransform = new Transform
                    (
                    transform.Position.X,
                    transform.Position.Y + i * (i * spacing)
                    );

                itemTransform.Origin = new Vector2f(0.5f, 0.5f);
                itemTransfrom.Size = itemSizes[i];

                Engine.Draw(spriteToDraw, itemTransform);
            }
        }
    */
        }
        
    
}
