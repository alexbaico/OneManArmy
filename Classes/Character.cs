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
        public int spriteMid;

        public Character(int[] position, int lives, string spritePath, int speed, Animation defaultAnimation, Animation attackAnimation, int spriteWidth) {
            this.position = position;
            this.lives = lives;
            this.sprite = Engine.LoadImage(spritePath);
            this.speed = speed;
            this.defaultAnimation = defaultAnimation;
            this.currentAnimation = defaultAnimation;
            this.attackAnimation = attackAnimation;
            this.spriteMid = spriteWidth / 2;
        }

        public void setSprite(string spritePath) {
            this.sprite = Engine.LoadImage(spritePath);
        }

        public void Render() {
            if(this.currentAnimation.Render(this.position[0] - spriteMid, this.position[1]))
            {
                this.currentAnimation = defaultAnimation;
            }
        }

        public void attack(bool right)
        {
            this.currentAnimation = attackAnimation;
            this.currentAnimation.rightDirection = right;
            this.currentAnimation.spritesCount = 0;
        }
       
    }


}
