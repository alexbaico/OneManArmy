using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Classes
{

    internal class Character
    {

        public int[] position = new int[2];
        public int lives;
        public int speed;
        public Animation currentAnimation;
        public Animation defaultAnimation;
        public Animation attackAnimation;
        public Animation hitAnimation;
        public Animation deathAnimation;
        public int spriteMid;
        private Animation[] atkEffects;

        public Character(int[] position, int lives, int speed, int spriteWidth, Animation defaultAnimation, Animation attackAnimation, Animation[] atkEffects, Animation hitAnimation, Animation deathAnimation) {
            this.position = position;
            this.lives = lives;
            this.speed = speed;
            this.defaultAnimation = defaultAnimation;
            this.currentAnimation = defaultAnimation;
            this.attackAnimation = attackAnimation;
            this.hitAnimation = hitAnimation;
            this.deathAnimation = deathAnimation;
            this.spriteMid = spriteWidth / 2;
            this.atkEffects = atkEffects;
        }

        public void ResetChar(int[] position, int lives, int speed) {
            this.position = position;
            this.lives = lives;
            this.speed = speed;
            this.defaultAnimation.rightDirection = speed > 0;
            this.currentAnimation.rightDirection = speed > 0;
            this.attackAnimation.rightDirection = speed > 0;
            this.hitAnimation.rightDirection = speed > 0;
            this.deathAnimation.rightDirection = speed > 0;

            this.defaultAnimation.spritesCount = 0;
            this.currentAnimation.spritesCount = 0;
            this.attackAnimation.spritesCount = 0;
            this.hitAnimation.spritesCount = 0;
            this.deathAnimation.spritesCount = 0;
            this.currentAnimation = defaultAnimation;
        }

        public bool Render() {
            if (currentAnimation == deathAnimation && currentAnimation.Render(this.position[0] - spriteMid, this.position[1])) { 
                return true;
            }
            if (currentAnimation == attackAnimation && atkEffects.Length > 0)
            {
                Random random = new Random();
                Animation effectAnimation = this.atkEffects[random.Next(atkEffects.Length)];
                effectAnimation.rightDirection = currentAnimation.rightDirection;
                effectAnimation.Render(this.position[0], this.position[1]);
            }
            if (this.currentAnimation.Render(this.position[0] - spriteMid, this.position[1]))
            {
                this.currentAnimation = defaultAnimation;
                return true;
            }
            return false;
        }

        public void attack(bool right)
        {
            this.currentAnimation = attackAnimation;
            this.currentAnimation.rightDirection = right;
            this.currentAnimation.spritesCount = 0;
        }

        internal void GetHit()
        {
            this.lives--;
            if (this.lives <= 0)
            {
                currentAnimation = deathAnimation;
            } else
            {
                currentAnimation = hitAnimation;
            }
            this.currentAnimation.spritesCount = 0;
        }
    }


}
