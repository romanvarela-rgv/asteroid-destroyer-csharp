using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineGDI
{
    class Character
    {
        public Transform transform;

        public float speed = 5f;
        public float velocityX = 0f;
        public float velocityY = 0f;

        public int direction = 1;

        public string texturePath = "player.png";

        //  INPUT
        public void Input()
        {
            velocityX = 0;
            velocityY = 0;

            if (Engine.IsKeyDown(Keys.A))
            {
                velocityX = -speed;
                direction = -1;
            }

            if (Engine.IsKeyDown(Keys.D))
            {
                velocityX = speed;
                direction = 1;
            }

            if (Engine.IsKeyDown(Keys.W))
            {
                velocityY = -speed;
            }

            if (Engine.IsKeyDown(Keys.S))
            {
                velocityY = speed;
            }
        }

        //  UPDATE
        public void Update()
        {
            transform.x += velocityX;
            transform.y += velocityY;
        }

        //  DRAW
        public void Draw()
        {
            float finalScaleX = direction == 1 ? transform.scaleX : -transform.scaleX;

            Engine.Draw(
                texturePath,
                transform.x,
                transform.y,
                finalScaleX,
                transform.scaleY,
                transform.angle
            );
        }
    }
}
