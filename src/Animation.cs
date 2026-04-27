using System;
using System.Collections.Generic;

namespace EngineGDI
{
    public class Animation
    {
        private bool isLoop;
        private string name;
        private float speed;
        private float currentTime;
        private int currentFrame = 0;
        private List<string> frames = new List<string>();

        public string CurrentFrame => frames.Count > 0 ? frames[currentFrame] : "";

        public Animation(string name, float speed, List<string> frames = null, bool isLoop = true)
        {
            this.name = name;
            this.speed = speed;
            this.isLoop = isLoop;
            this.currentFrame = 0;
            this.currentTime = 0;

            if (frames != null)
            {
                this.frames = frames;
            }
        }

        public void Update(float deltaTime)
        {
            if (frames.Count == 0) return;

            currentTime += speed * deltaTime;

            if (currentTime >= 1f)
            {
                currentTime = 0;
                currentFrame++;

                if (currentFrame >= frames.Count)
                {
                    if (isLoop)
                        currentFrame = 0;
                    else
                        currentFrame = frames.Count - 1;
                }
            }
        }

        public void Reset()
        {
            currentFrame = 0;
            currentTime = 0;
        }
    }
}
