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
        public Image sprite;
        public Animation currentAnimation;
        public Animation defaultAnimation;
        public Animation attackAnimation;
        public Animation hitAnimation;
        public Animation deathAnimation;
        public int spriteMid;
        private Animation[] atkEffects;

        public Character(int[] position, int lives, string spritePath, int speed, Animation defaultAnimation, Animation attackAnimation, int spriteWidth, Animation[] atkEffects, Animation hitAnimation, Animation deathAnimation) {
            this.position = position;
            this.lives = lives;
            this.sprite = Engine.LoadImage(spritePath);
            this.speed = speed;
            this.defaultAnimation = defaultAnimation;
            this.currentAnimation = defaultAnimation;
            this.attackAnimation = attackAnimation;
            this.hitAnimation = hitAnimation;
            this.deathAnimation = deathAnimation;
            this.spriteMid = spriteWidth / 2;
            this.atkEffects = atkEffects;
        }

        public void setSprite(string spritePath) {
            this.sprite = Engine.LoadImage(spritePath);
        }

        public bool Render() {
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
