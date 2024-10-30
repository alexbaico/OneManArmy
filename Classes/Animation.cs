using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Classes
{
    internal class Animation
    {
        public int amountSprites;
        public Image[] spritesRight;
        public Image[] spritesLeft;
        public int spritesCount;
        public int spritesRotationTime;
        public bool rightDirection;
        public bool loop;
        public int animDuration;

        public Animation(string folder, string spriteName, int spritesAmount, bool loop, int animDuration) {
            this.spritesRight = new Image[spritesAmount];
            this.spritesLeft = new Image[spritesAmount];
            for (int i = 1; i <= spritesAmount; i++) {
                spritesRight[i-1] = Engine.LoadImage(folder + "/" + spriteName + "R" + i +".png");
                spritesLeft[i-1] = Engine.LoadImage(folder + "/" + spriteName + "L" + i+".png");
            }
            this.spritesCount = 0;
            this.loop = loop;
            this.animDuration = animDuration;
            spritesRotationTime = animDuration / Program.delay / spritesAmount;
        }

        public bool Render(int x, int y) {
            spritesRotationTime -= Program.delay;
            if (spritesRotationTime <= 0)
            {
                spritesCount++;
                spritesRotationTime = animDuration / Program.delay / spritesRight.Length;
            }
            if (spritesCount == spritesRight.Length)
            {
                if (loop)
                {
                    spritesCount = 0;
                } else
                {
                    spritesCount = 0;
                    return true;
                }
            }
            Engine.Draw(rightDirection ? spritesRight[spritesCount] : spritesLeft[spritesCount], x, y);
            return false;
        }

    }
}
