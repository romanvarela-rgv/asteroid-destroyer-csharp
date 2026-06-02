using System;
using System.Collections.Generic;

namespace EngineGDI
{
    public class AsteroidFactory 
    {
        private Random random = new Random();
        private float safeRadius = 200f;

        public Asteroid CreateSafeAsteroid(Vector2f playerPosition, int screenWidth, int screenHeight)
        {
            Vector2f spawnPosition = new Vector2f(0, 0);
            bool validPosition = false;

            while (!validPosition)
            {
                spawnPosition.X = random.Next(0, screenWidth);
                spawnPosition.Y = random.Next(0, screenHeight);

                float distanceToPLayer = Vector2f.Distance(spawnPosition, playerPosition);

                if (distanceToPLayer > safeRadius)
                {
                    validPosition = true;
                }
            }

            Vector2f velocity = new Vector2f(
                (float)(random.NextDouble() * 100 - 50),
                (float)(random.NextDouble() * 100 - 50)
                );

            string spritePath = $"assets/textures/Animations/Idle_e/meteorito{random.Next(1, 11)}.png";

            float angle = (float)(random.NextDouble() * 360);
            float rotationSpeed = (float)(random.NextDouble() * 100 - 50);

            float scale = (float)(random.NextDouble() * 0.5 + 0.5);

            return new Asteroid(spawnPosition, velocity, spritePath, angle, rotationSpeed, scale);
        }
    }
    
}
