using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineGDI
{
    public class Player
    {
        public int posX;
        public int posY;

        private string sprite;

        public string Sprite => sprite;
        

        public Player(string sprite, int posX, int posY) 
        { 
            this.sprite = sprite;
            this.posX = posX;
            this.posY = posY;
        }
    }
}
