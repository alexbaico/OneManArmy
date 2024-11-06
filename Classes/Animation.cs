﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Classes
{


    internal class Animation
    {
        public Image[] spritesRight;
        public Image[] spritesLeft;
        public int spritesCount;
        public int spritesRotationTime;
        public bool rightDirection;
        public bool loop;
        public int animDuration;
        public int[] offsetX;
        public int offsetY;

        public Animation(Image[] spritesRight, Image[] spritesLeft, bool loop, int animDuration, int[] offsetX, int offsetY, bool rightDirection) {
            this.spritesRight = spritesRight;
            this.spritesLeft = spritesLeft;
            this.spritesCount = 0;
            this.loop = loop;
            this.animDuration = animDuration;
            spritesRotationTime = animDuration / Program.delay / spritesRight.Length;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.rightDirection = rightDirection;
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
                spritesCount = 0;
                if (!loop)
                {
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
