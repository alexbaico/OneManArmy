﻿using System;
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
        public int[] offsetX;
        public int offsetY;

        public Animation(string folder, string spriteName, int spritesAmount, bool loop, int animDuration, int[] offsetX, int offsetY) {
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
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public void resetAnimation() { 
            this.spritesCount = 0;
            this.rightDirection = true;
            spritesRotationTime = animDuration / Program.delay / spritesRight.Length;
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
            if (rightDirection)
            {
                Engine.Draw(spritesRight[spritesCount], x + offsetX[1], y + offsetY);
            }
            else 
            { 
                Engine.Draw(spritesLeft[spritesCount], x + offsetX[0], y + offsetY);
            }
            return false;
        }

    }
}
