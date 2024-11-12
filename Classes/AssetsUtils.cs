using NAudio.Wave;
using System.IO;
using System.Media;

namespace OneManArmy.Classes
{
    public static class AssetsUtils
    {
        public static Assets assets;


        public struct Assets {
            public Image menuBackground;
            public Image winBackground;
            public Image loseBackground;

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

            public WaveOut menuMusic;
            public Mp3FileReader menuMusicFR;
            public WaveOut winMusic;
            public Mp3FileReader winMusicFR;
            public WaveOut gameplayMusic;
            public WaveOut loseMusic;

            public SoundPlayer[] hitSoundEffects;
            public SoundPlayer[] missSoundEffects;
            public SoundPlayer[] painSoundEffects;
            public SoundPlayer[] deathSoundEffects;


            public Image[][] companionIdleImages;
            public Image[] birdLImages;
            public Image[] birdRImages;

        }

        static AssetsUtils()
        {
            assets.menuBackground = Engine.LoadImage("assets/backs/menu/menu.png");
            assets.winBackground = Engine.LoadImage("assets/backs/win/win.png");
            assets.loseBackground = Engine.LoadImage("assets/backs/lose/lose.png");

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

            assets.enemiesIdleRImages = new Image[Program.enemiesAmount][];
            assets.enemiesIdleLImages = new Image[Program.enemiesAmount][];
            assets.enemiesDeathRImages = new Image[Program.enemiesAmount][];
            assets.enemiesDeathLImages = new Image[Program.enemiesAmount][];
            assets.enemiesWalkRImages = new Image[Program.enemiesAmount][];
            assets.enemiesWalkLImages = new Image[Program.enemiesAmount][];
            assets.enemiesAtkRImages = new Image[Program.enemiesAmount][];
            assets.enemiesAtkLImages = new Image[Program.enemiesAmount][];

            assets.menuMusicFR = new Mp3FileReader("assets/sounds/music/menu/menu.mp3");
            assets.menuMusic = new WaveOut();

            assets.winMusicFR = new Mp3FileReader("assets/sounds/music/win/win.mp3");
            assets.winMusic = new WaveOut();
            assets.winMusic = new WaveOut();

            assets.gameplayMusic = new WaveOut();
            assets.gameplayMusic.Volume = 0.5f;
            assets.loseMusic = new WaveOut();

            assets.hitSoundEffects = new SoundPlayer[5];
            assets.missSoundEffects = new SoundPlayer[5];
            assets.painSoundEffects = new SoundPlayer[8];
            assets.deathSoundEffects = new SoundPlayer[2];

            assets.companionIdleImages = new Image[2][];
            assets.birdLImages = new Image[4];
            assets.birdRImages = new Image[4];

            for (int i = 1; i <= 8; i++)
            {
                if (i == 1)
                {
                    assets.playerAttkREffects[i - 1] = new Image[8];
                    assets.playerAttkLEffects[i - 1] = new Image[8];

                    for (int j = 1; j <= 8; j++)
                    {
                        assets.playerAttkREffects[i - 1][j - 1] = Engine.LoadImage("assets/effects/atk1/atk" + "R" + j + ".png");
                        assets.playerAttkLEffects[i - 1][j - 1] = Engine.LoadImage("assets/effects/atk1/atk" + "L" + j + ".png");
                    }


                }

                if (i <= 2)
                {
                    assets.deathSoundEffects[i - 1] = new SoundPlayer("assets/sounds/death/death" + i + ".wav");

                    assets.companionIdleImages[i - 1] = new Image[4];

                    for (int j = 1; j <= 4; j++)
                    {
                        assets.companionIdleImages[i - 1][j - 1] = Engine.LoadImage("assets/animals/comp/animal"+i+"/idle" + j + ".png");
                    }
                }
                if (i <= Program.enemiesAmount)
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
                    assets.playerIdleRImages[i - 1] = Engine.LoadImage("assets/spearguy/idle/idle" + "R" + i + ".png");
                    assets.playerIdleLImages[i - 1] = Engine.LoadImage("assets/spearguy/idle/idle" + "L" + i + ".png");
                    assets.playerHitRImages[i - 1] = Engine.LoadImage("assets/spearguy/hit/hit" + "R" + i + ".png");
                    assets.playerHitLImages[i - 1] = Engine.LoadImage("assets/spearguy/hit/hit" + "L" + i + ".png");

                    assets.playerDeathRImages[i - 1] = Engine.LoadImage("assets/spearguy/death/death" + "R" + i + ".png");
                    assets.playerDeathLImages[i - 1] = Engine.LoadImage("assets/spearguy/death/death" + "L" + i + ".png");

                    //atk animations are 8 images long
                    assets.playerAtkRImages[i - 1] = Engine.LoadImage("assets/spearguy/attack/spear" + "R" + i + ".png");
                    assets.playerAtkRImages[i + 3] = Engine.LoadImage("assets/spearguy/attack/spear" + "R" + (i + 3) + ".png");
                    assets.playerAtkLImages[i - 1] = Engine.LoadImage("assets/spearguy/attack/spear" + "L" + i + ".png");
                    assets.playerAtkLImages[i + 3] = Engine.LoadImage("assets/spearguy/attack/spear" + "L" + (i + 3) + ".png");

                    assets.birdLImages[i - 1] = Engine.LoadImage("assets/animals/bird/fly" + "L" + i + ".png");
                    assets.birdRImages[i - 1] = Engine.LoadImage("assets/animals/bird/fly" + "R" + i + ".png");
                }

                if (i <= 5)
                {
                    assets.hitSoundEffects[i - 1] = new SoundPlayer("assets/sounds/hit/hit" + i + ".wav");
                    assets.missSoundEffects[i - 1] = new SoundPlayer("assets/sounds/miss/miss" + i + ".wav");
                }

                if (i <= 6)
                {
                    assets.playerDeathRImages[i - 1] = Engine.LoadImage("assets/spearguy/death/death" + "R" + i + ".png");
                    assets.playerDeathLImages[i - 1] = Engine.LoadImage("assets/spearguy/death/death" + "L" + i + ".png");
                }

                assets.painSoundEffects[i - 1] = new SoundPlayer("assets/sounds/pain/pain" + i + ".wav");

            }

        }

        public static void PlayMenuMusic()
        {
            assets.menuMusicFR.Seek(0, SeekOrigin.Begin);
            assets.menuMusic.Init(assets.menuMusicFR);
            assets.menuMusic.Play();
        }
        public static void StopMenuMusic()
        {
            assets.menuMusic.Stop();
            assets.menuMusic.Dispose();
        }
        public static void PlayWinMusic()
        {
            assets.winMusicFR.Seek(0, SeekOrigin.Begin);
            assets.winMusic.Init(assets.winMusicFR);
            assets.winMusic.Play();
        }
        public static void StopWinMusic()
        {
            assets.winMusic.Stop();
            assets.winMusic.Dispose();
        }

        public static void PlayGameplayMusic()
        {
            Mp3FileReader fr = new Mp3FileReader("assets/sounds/music/gameplay/gameplay"+ (Program.random.Next(2) + 1) + ".mp3");
            assets.gameplayMusic.Init(fr);
            assets.gameplayMusic.Play();
        }
        public static void StopGameplayMusic()
        {
            assets.gameplayMusic.Stop();
            assets.gameplayMusic.Dispose();
        }

        public static void PlayLoseMusic()
        {
            Mp3FileReader fr = new Mp3FileReader("assets/sounds/music/lose/lose" + (Program.random.Next(3) + 1)+".mp3");
            assets.loseMusic.Init(fr);
            assets.loseMusic.Play();
        }
        public static void StopLoseMusic()
        {
            assets.loseMusic.Stop();
            assets.loseMusic.Dispose();
        }


    }
}
