using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Classes
{
    public static class AssetsUtils
    {
        public static Assets assets;

        public struct Assets {
            public Image[] playerIdleRImages;
            public Image[] playerIdleLImages;
            public Image[] playerAtkRImages;
            public Image[] playerAtkLImages;
            public Image[] playerHitRImages;
            public Image[] playerHitLImages;
            public Image[] playerDeathRImages;
            public Image[] playerDeathLImages;
            public Image[][] playerAttkREffects;
            public Image[][] playerAttkLEffects;
             
            public Image[][] enemiesIdleRImages;
            public Image[][] enemiesIdleLImages;
            public Image[][] enemiesAtkRImages;
            public Image[][] enemiesAtkLImages;
            public Image[][] enemiesHitImages;
            public Image[][] enemiesDeathRImages;
            public Image[][] enemiesDeathLImages;
            public Image[][] enemiesWalkRImages;
            public Image[][] enemiesWalkLImages;
            public Image[][] enemyAttkEffects;

        }

        static AssetsUtils()
        {
            assets.playerIdleRImages = new Image[4];
            assets.playerIdleLImages = new Image[4];
            assets.playerHitRImages = new Image[4];
            assets.playerHitLImages = new Image[4];
            assets.playerAtkRImages = new Image[8];
            assets.playerAtkLImages = new Image[8];
            assets.playerDeathRImages = new Image[6];
            assets.playerDeathLImages = new Image[6];

            assets.playerAttkREffects = new Image[1][];
            assets.playerAttkLEffects = new Image[1][];

            assets.enemiesIdleRImages = new Image[2][];
            assets.enemiesIdleLImages = new Image[2][];
            assets.enemiesDeathRImages = new Image[2][];
            assets.enemiesDeathLImages = new Image[2][];
            assets.enemiesWalkRImages = new Image[2][];
            assets.enemiesWalkLImages = new Image[2][];
            assets.enemiesAtkRImages = new Image[2][];
            assets.enemiesAtkLImages = new Image[2][];

            for (int i = 1; i <= 6; i++)
            {
                if (i <= 2)
                {
                    //enemies
                    assets.enemiesIdleRImages[i - 1] = new Image[4];
                    assets.enemiesIdleLImages[i - 1] = new Image[4];

                    assets.enemiesDeathRImages[i - 1] = new Image[5];
                    assets.enemiesDeathLImages[i - 1] = new Image[5];

                    assets.enemiesWalkRImages[i - 1] = new Image[6];
                    assets.enemiesWalkLImages[i - 1] = new Image[6];

                    assets.enemiesAtkRImages[i - 1] = new Image[8];
                    assets.enemiesAtkLImages[i - 1] = new Image[8];

                    for (int j = 1; j <= 4; j++)
                    {
                        assets.enemiesIdleRImages[i - 1][j - 1] = Engine.LoadImage("assets/enemies/enemy" + i + "/idle/idle" + "R" + j + ".png");
                        assets.enemiesIdleLImages[i - 1][j - 1] = Engine.LoadImage("assets/enemies/enemy" + i + "/idle/idle" + "L" + j + ".png");

                        assets.enemiesAtkRImages[i - 1][j - 1] = Engine.LoadImage("assets/enemies/enemy" + i + "/atk/atk" + "R" + j + ".png");
                        assets.enemiesAtkLImages[i - 1][j - 1] = Engine.LoadImage("assets/enemies/enemy" + i + "/atk/atk" + "L" + j + ".png");
                        assets.enemiesAtkRImages[i - 1][j + 3] = Engine.LoadImage("assets/enemies/enemy" + i + "/atk/atk" + "R" + (j + 4) + ".png");
                        assets.enemiesAtkLImages[i - 1][j + 3] = Engine.LoadImage("assets/enemies/enemy" + i + "/atk/atk" + "L" + (j + 4) + ".png");
                    }
                    for (int j = 1; j <= 5; j++)
                    {
                        assets.enemiesDeathRImages[i - 1][j - 1] = Engine.LoadImage("assets/enemies/enemy" + i + "/death/death" + "R" + j + ".png");
                        assets.enemiesDeathLImages[i - 1][j - 1] = Engine.LoadImage("assets/enemies/enemy" + i + "/death/death" + "L" + j + ".png");
                    }
                    for (int j = 1; j <= 6; j++)
                    {
                        assets.enemiesWalkRImages[i - 1][j - 1] = Engine.LoadImage("assets/enemies/enemy" + i + "/walk/walk" + "R" + j + ".png");
                        assets.enemiesWalkLImages[i - 1][j - 1] = Engine.LoadImage("assets/enemies/enemy" + i + "/walk/walk" + "L" + j + ".png");

                    }
                }


                if (i <= 4) 
                { 
                    assets.playerIdleRImages[i - 1] = Engine.LoadImage("assets/spearguy/default/idle" + "R" + i + ".png");
                    assets.playerIdleLImages[i - 1] = Engine.LoadImage("assets/spearguy/default/idle" + "L" + i + ".png");
                    assets.playerHitRImages[i - 1] = Engine.LoadImage("assets/spearguy/hit/hit" + "R" + i + ".png");
                    assets.playerHitLImages[i - 1] = Engine.LoadImage("assets/spearguy/hit/hit" + "L" + i + ".png");

                    assets.playerDeathRImages[i - 1] = Engine.LoadImage("assets/spearguy/death/death" + "R" + i + ".png");
                    assets.playerDeathLImages[i - 1] = Engine.LoadImage("assets/spearguy/death/death" + "L" + i + ".png");

                    //atk animations are 8 images long
                    assets.playerAtkRImages[i - 1] = Engine.LoadImage("assets/spearguy/attack/spear" + "R" + i + ".png");
                    assets.playerAtkRImages[i + 3] = Engine.LoadImage("assets/spearguy/attack/spear" + "R" + (i + 3) + ".png");
                    assets.playerAtkLImages[i - 1] = Engine.LoadImage("assets/spearguy/attack/spear" + "L" + i + ".png");
                    assets.playerAtkLImages[i + 3] = Engine.LoadImage("assets/spearguy/attack/spear" + "L" + (i + 3) + ".png");

                }
                else
                {
                    assets.playerDeathRImages[i - 1] = Engine.LoadImage("assets/spearguy/death/death" + "R" + i + ".png");
                    assets.playerDeathLImages[i - 1] = Engine.LoadImage("assets/spearguy/death/death" + "L" + i + ".png");



                }

                if (i == 1)
                {
                    assets.playerAttkREffects[i - 1] = new Image[8];
                    assets.playerAttkLEffects[i - 1] = new Image[8];
                    for(int j = 1; j <= 8; j++)
                    {
                        assets.playerAttkREffects[i-1][j-1] = Engine.LoadImage("assets/effects/atk1/atk" + "R" + j + ".png");
                        assets.playerAttkLEffects[i-1][j-1] = Engine.LoadImage("assets/effects/atk1/atk" + "L" + j + ".png");
                    }

                }

            }
        }


    }
}
