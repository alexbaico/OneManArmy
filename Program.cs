using MyGame.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using Tao.Sdl;
using NAudio;
using NAudio.Wave;

namespace MyGame
{

    class Program
    {
        
        static int screenWidth = 1024;
        static int screenHeight = 1024;
        static int gameplayScreenWidth = 1280;
        static int gameplayScreenHeight = 689;

        public static Random random = new Random();
        static int mouseX = 0;
        static int mouseY = 0;

        //Assets
        static Image gameplayBackgrounds;
        static Image menuBackground = Engine.LoadImage("assets/backs/menu/menu.png");
        static Image winBackground = Engine.LoadImage("assets/backs/win/win.png");
        static Image loseBackground = Engine.LoadImage("assets/backs/lose/lose.png");
        
        static Font fuente;


        static Image[] meterL;
        static Image[] meterR;
        static Image[] meterLives;

        //GameState vars (things updated during gameplay)
        static int meterLCounter = 0;
        static int meterRCounter = 0;
        static int gameState = 0;
        static int enemySpawnTime = 550;
        static int cooldown = 0;
        static bool pressed = false;
        static bool playerHitted = false;
        static int playerHittedTime = 1;
        static int enemiesKillCount = 0;

        static DateTime startTime;

        static DateTime lastTime = DateTime.Now;
        public static int lastFrameTime = 0;

        static Character enemyOnTheLeft = null;
        static Character enemyOnTheRight = null;

        static Character player;
        static List<Character> enemies = new List<Character>();
        static List<Character> deadEnemies = new List<Character>();
        static List<Character> inactiveEnemies = new List<Character>();

        static int tutorialStep = 0;


        //Game config (constants or configs of the game)
        static int enemiesObjective = 100;
        static int delay = 10;
        static int spawnTime = 2000;
        static int enemyBaseSpeed = 7;
        static int distanceToPunch = 175;
        static int cooldownTime = 50;
        static int cooldownMissTime = 350;

        //Gameplay options
        static bool tutorial = true;
        static int difficulty = 1;
        static int gameMode = 1;

        static void Main(string[] args)
        {
            Engine.Initialize(screenWidth, screenHeight);

            initializeMeters(out meterL, out meterR, out meterLives);
            initializePlayer();

            AssetsUtils.PlayMenuMusic();

            fuente = Engine.LoadFont("assets/light_pixel-7.ttf", 25);
            gameplayBackgrounds = Engine.LoadImage("assets/backs/" + (random.Next(7) + 1) + ".jpg");

            while (true)
            {
                CheckInputs();
                Update();
                Render();
                Sdl.SDL_Delay(delay);
            }
        }

        private static void initializePlayer()
        {
            Animation[] attkEffects = new Animation[] { new Animation(AssetsUtils.assets.playerAttkREffects[0], AssetsUtils.assets.playerAttkLEffects[0], false, 200, new int[] { -170, 86 }, 10, false) };
            player = new Character(new int[] { 650, 552 }, 4, 0, 150, new Animation(AssetsUtils.assets.playerIdleRImages, AssetsUtils.assets.playerIdleLImages, true, 600, new int[] { 0, 0 }, 0, false), 
                new Animation(AssetsUtils.assets.playerAtkRImages, AssetsUtils.assets.playerAtkLImages, false, 200, new int[] { 0, 0 }, 0, false), attkEffects, new Animation(AssetsUtils.assets.playerHitRImages, AssetsUtils.assets.playerHitLImages, false, 400, new int[] { 0, 0 }, 0, false), new Animation(AssetsUtils.assets.playerDeathRImages, AssetsUtils.assets.playerDeathLImages, false, 1000, new int[] { 0, 0 }, 0, false));
        }

        //This are the bars that shows when you can hit an enemy / an enemy is in hit range
        private static void initializeMeters(out Image[] meterL, out Image[] meterR, out Image[] meterLives)
        {
            meterL = new Image[11];
            meterR = new Image[11];
            meterLives = new Image[5];
            for (int i = 0; i <= 10; i++)
            {
                meterL[i] = Engine.LoadImage("assets/UI/meterL" + i + ".png");
                meterR[i] = Engine.LoadImage("assets/UI/meterR" + i + ".png");
            }
            for (int i = 0; i < 5; i++)
            {
                meterLives[i] = Engine.LoadImage("assets/UI/livesMeter" + i + ".png");
            }
        }

        static void CheckInputs()
        {

            //Change from menus pressing Space
            if (Engine.KeyPress(Engine.KEY_SPACE) && !pressed)
            {
                if (gameState == 1 && tutorial)
                {
                    if (tutorialStep == 0 || tutorialStep == 1 || tutorialStep == 2 || tutorialStep == 3)
                    {
                        tutorialStep++;
                    }
                }

                if (gameState == 0)
                {
                    AssetsUtils.StopMenuMusic();
                    AssetsUtils.PlayGameplayMusic();
                    gameState = 1;
                    Engine.Initialize(gameplayScreenWidth, gameplayScreenHeight);
                    if (!tutorial)
                    {
                        startTime = DateTime.Now;
                    }
                }
                if (gameState == 2 || gameState == 3)
                {
                    AssetsUtils.StopLoseMusic();
                    AssetsUtils.StopWinMusic();
                    AssetsUtils.PlayMenuMusic();

                    ResetGame();
                    gameState = 0;
                }


                pressed = true; 
            }
            
            //Action keys
            if (cooldown <= 0 && player.lives > 0 && playerHittedTime - 200 < 0)
            {
                if ((Engine.KeyPress(Engine.KEY_LEFT) || Engine.KeyPress(Engine.KEY_A) || Engine.MouseClick(Engine.MOUSE_LEFT, out mouseX, out mouseY)) && !pressed)
                {
                    AttackLeft();
                    pressed = true;
                }

                if ((Engine.KeyPress(Engine.KEY_RIGHT) || Engine.KeyPress(Engine.KEY_D) || Engine.MouseClick(Engine.MOUSE_RIGHT, out mouseX, out mouseY)) && !pressed)
                {
                    AttackRight();
                    pressed = true;
                }

            }

            //Press flag check
            if (!(Engine.KeyPress(Engine.KEY_LEFT) || Engine.KeyPress(Engine.KEY_A) || Engine.KeyPress(Engine.KEY_RIGHT) || Engine.KeyPress(Engine.KEY_D) || Engine.KeyPress(Engine.KEY_SPACE) || Engine.KeyPress(Engine.KEY_T)))
            {
                pressed = false;
            }

            //Exit game
            if (Engine.KeyPress(Engine.KEY_ESC))
            {
                Environment.Exit(0);
            }

            //Main menu options inputs
            if (gameState == 0) {
                if (Engine.KeyPress(Engine.KEY_1))
                {
                    difficulty = 1;
                }
                if (Engine.KeyPress(Engine.KEY_2))
                {
                    difficulty = 2;
                }
                if (Engine.KeyPress(Engine.KEY_3))
                {
                    difficulty = 3;
                }
                if (Engine.KeyPress(Engine.KEY_C))
                {
                    gameMode = 1;
                }
                if (Engine.KeyPress(Engine.KEY_I))
                {
                    gameMode = 2;
                }
                if (Engine.KeyPress(Engine.KEY_T) && !pressed)
                {
                    tutorial = !tutorial;
                    pressed = true;
                }
            }
        }

        private static void ResetGame()
        {
            enemies = new List<Character>();
            deadEnemies = new List<Character>();
            inactiveEnemies = new List<Character>();
            player.currentAnimation = player.defaultAnimation;
            player.defaultAnimation.spritesCount = 0;
            cooldown = 0;
            player.lives = 4;
            enemiesKillCount = 0;
            pressed = false;
            spawnTime = 2000;
            meterRCounter = 0;
            meterLCounter = 0;
            tutorialStep = 0;
            gameplayBackgrounds = Engine.LoadImage("assets/backs/" + (random.Next(7) + 1) + ".jpg");
        }

        //Attack right action
        private static void AttackRight()
        {
            player.attack(true);

            if (enemyOnTheRight != null)
            {
                enemyOnTheRight.lives--;
                if (enemyOnTheRight.lives <= 0)
                {
                    deadEnemies.Add(enemyOnTheRight);
                    enemyOnTheRight.GetHit();
                    enemies.Remove(enemyOnTheRight);
                    enemyOnTheRight = null;
                    if (!tutorial)
                    {
                        enemiesKillCount++;
                    }
                }
                cooldown = cooldownTime;
                if (tutorial)
                {
                    tutorial = false;
                    startTime = DateTime.Now;
                }
            }
            else
            {
                AssetsUtils.assets.missSoundEffects[random.Next(5)].Play();
                cooldown = cooldownMissTime;
            }
        }


        //Attack left action
        private static void AttackLeft()
        {

            player.attack(false);

            if (enemyOnTheLeft != null)
            {
                enemyOnTheLeft.lives--;
                if (enemyOnTheLeft.lives <= 0)
                {
                    deadEnemies.Add(enemyOnTheLeft);
                    enemyOnTheLeft.GetHit();
                    enemies.Remove(enemyOnTheLeft);
                    enemyOnTheLeft = null;
                    if (!tutorial)
                    {
                        enemiesKillCount++;
                    }
                }
                cooldown = cooldownTime;

            }
            else
            {
                cooldown = cooldownMissTime;
                AssetsUtils.assets.missSoundEffects[random.Next(5)].Play();
            }
        }

        static void Update()
        {
            if (gameState == 1) {

                if (enemiesKillCount >= enemiesObjective && gameMode == 1)
                {
                    gameState = 3;

                    AssetsUtils.StopGameplayMusic();
                    AssetsUtils.PlayWinMusic();
                    Engine.Initialize(screenWidth, screenHeight);
                }

                lastFrameTime = (int)(DateTime.Now - lastTime).TotalMilliseconds;
                lastTime = DateTime.Now;

                inactiveEnemies.ForEach(enemy => deadEnemies.Remove(enemy));

                enemyOnTheLeft = enemies.Where(enemy => player.position[0] - distanceToPunch < enemy.position[0] && player.position[0] > enemy.position[0]).FirstOrDefault();
                enemyOnTheRight = enemies.Where(enemy => player.position[0]  + distanceToPunch > enemy.position[0] && player.position[0] < enemy.position[0]).FirstOrDefault();

                SpawnEnemy();

                MoveEnemies();
            
                EnemyHitsPlayer();
            }

        }

        //Moving enemies
        private static void MoveEnemies()
        {
            if (player.lives > 0 && (!tutorial || (enemyOnTheLeft == null && enemyOnTheRight == null && tutorialStep != 0 && tutorialStep != 1 && tutorialStep != 2 && tutorialStep != 3)))
            {
                enemies.ForEach(enemy =>
                {
                    if (playerHitted && playerHittedTime > 0)
                    {
                        enemy.position[0] -= enemy.speed / 2;
                    }
                    else
                    {
                        enemy.position[0] += enemy.speed;
                    }
                });
            }
        }

        //Spawn enemy logic
        private static void SpawnEnemy()
        {
            if (cooldown > 0)
                cooldown -= lastFrameTime;
            if (playerHitted) 
                return;
            if (spawnTime > 0)
                spawnTime -= lastFrameTime;

            /* spawn enemies when spawnTime reaches 0, player is alive. Stop spawning while tutorial messages are being shown */
            if (spawnTime <= 0 && player.lives > 0 && (!tutorial || (enemyOnTheLeft == null && enemyOnTheRight == null && tutorialStep != 0 && tutorialStep != 1 && tutorialStep != 2 && tutorialStep != 3)))
            {
                /* random appearance condition logic */
                spawnTime = enemySpawnTime + (random.Next(2) == 0 ? random.Next(150) : (random.Next(250) * -1)) - 50 * difficulty;
                int xPos = random.Next(2) == 0 ? (0 - 100) : (gameplayScreenWidth + 100);
                if (tutorial && enemies.Count < 2)
                {
                    if(enemies.Count < 1)
                    {
                        xPos = 0 - 100;
                    } else
                    {
                        xPos = gameplayScreenWidth + 100;
                    }
                }
                int enemySpeed = enemyBaseSpeed / 2 * (xPos <= 0 ? 1 : -1) * difficulty; //Enemy speed direction
                int enemyRandomAsset = random.Next(2);
                int reuseRandom = random.Next(2); // So not the same assets are being reused every time (a loop between dead enemies is generated once a few die)
                if ((inactiveEnemies.Count > 0 && reuseRandom == 0) || inactiveEnemies.Count > 20) // Ok... after 20 enemies in the inactive bag, always reuse one
                {
                    Character enemy = inactiveEnemies.First();
                    enemy.ResetChar(new int[] { xPos, player.position[1] }, 1, enemySpeed);
                    enemies.Add(enemy);
                    inactiveEnemies.Remove(enemy);
                } else
                {
                    enemies.Add(new Character(new int[] { xPos, player.position[1] }, 1, enemySpeed, 150, new Animation(AssetsUtils.assets.enemiesWalkRImages[enemyRandomAsset], AssetsUtils.assets.enemiesWalkLImages[enemyRandomAsset], true, 500, new int[] { 0, 0 }, 0, enemySpeed > 0), new Animation(AssetsUtils.assets.enemiesAtkRImages[enemyRandomAsset], AssetsUtils.assets.enemiesAtkLImages[enemyRandomAsset], false, 500, new int[] { 0, 0 }, 0, enemySpeed > 0), new Animation[] { },
                    new Animation(AssetsUtils.assets.enemiesIdleRImages[enemyRandomAsset], AssetsUtils.assets.enemiesIdleLImages[enemyRandomAsset], false, 500, new int[] { 0, 0 }, 0, enemySpeed > 0), new Animation(AssetsUtils.assets.enemiesDeathRImages[enemyRandomAsset], AssetsUtils.assets.enemiesDeathLImages[enemyRandomAsset], false, 500, new int[] { 0, 0 }, 0, enemySpeed > 0)));
                }
            }
        }

        //Player is hit by an enemy
        static void EnemyHitsPlayer() {
            Character enemyHitsPlayer = enemies.Where(enemy => enemy.position[0] < player.position[0] + 45 && enemy.position[0] > player.position[0] - 45).FirstOrDefault();
            if (enemyHitsPlayer != null && !playerHitted)
            {
                player.GetHit(true);
                enemyHitsPlayer.attack(enemyHitsPlayer.speed > 0);
                playerHitted = true;
                playerHittedTime = 600;
            }

            if (playerHitted)
            {
                playerHittedTime -= lastFrameTime;
                if (playerHittedTime <= 0)
                {
                    playerHitted = false;
                }
            }

        }

        static void Render()
        {
            Engine.Clear();
            switch (gameState)
            {
                case 1:
                    {
                        Engine.Draw(gameplayBackgrounds, 0, 0);
                        //Draw player
                        bool playerRender = player.Render();
                        //Player dies
                        if (player.lives <= 0 && playerRender)
                        {
                            gameState = 2;
                            AssetsUtils.StopGameplayMusic();
                            AssetsUtils.PlayLoseMusic();
                            Engine.Initialize(screenWidth, screenHeight);
                        }

                        enemies.ForEach(enemy =>
                        {
                            //Draw enemies
                            enemy.Render();

                            //DEBUG ENEMY POSITION
                            //Engine.DrawText("" + enemy.position[0], enemy.position[0], enemy.position[1] - 80, 250, 185, 15, fuente);
                        });
                        deadEnemies.ForEach(enemy =>
                        {
                            //Draw dead enemies
                            if (enemy.Render()) {
                                inactiveEnemies.Add(enemy);
                            }

                            //DEBUG ENEMY POSITION
                            //Engine.DrawText("" + enemy.position[0], enemy.position[0], enemy.position[1] - 80, 250, 185, 15, fuente);
                        });

                        RenderMeters();

                        //Draw Score or Survived time
                        if (gameMode == 1)
                        {
                            Engine.DrawText("Enemies killed: " + enemiesKillCount + " / " + enemiesObjective, player.position[0] - 100, 10, 250, 185, 15, fuente);
                        }
                        else
                        {
                            TimeSpan timeSurvived = DateTime.Now - startTime;
                            Engine.DrawText("Time survived: " + (tutorial ? "00:00:00" : 
                                ((timeSurvived.Hours < 10 ? "0"+ timeSurvived.Hours : "" + timeSurvived.Hours) + ":" +
                                (timeSurvived.Minutes < 10 ? "0" + timeSurvived.Minutes : ""+timeSurvived.Minutes) + ":" + 
                                (timeSurvived.Seconds < 10 ? "0"+ timeSurvived.Seconds : "" + timeSurvived.Seconds))), player.position[0] - 100, 10, 250, 185, 15, fuente);
                        }

                        //DEBUG lastFrameTime
                        //Engine.DrawText("timeBetween: " + lastFrameTime, 0, 100 , 250, 185, 15, fuente);
                        //DEBUG SPAWNTIME
                        //Engine.DrawText("" + spawnTime, 0, 100, 250, 185, 15, fuente);
                        //DEBUG COOLDOWN
                        //Engine.DrawText("" + cooldown, player.position[0], player.position[1] - 50, 250, 185, 15, fuente);

                        if (tutorial)
                        {
                            DrawTutorialMessages();
                        }

                        break;
                    }
                case 2:
                    {
                        Engine.Draw(loseBackground, 0, 0);
                        Engine.DrawText("Press SPACE BAR to continue", 280, 770, 255, 55, 50, fuente);
                        break;
                    }
                case 3:
                    {
                        Engine.Draw(winBackground, 0, 0);
                        Engine.DrawText("Press SPACE BAR to continue", 280, 870, 255, 55, 50, fuente);
                        break;
                    }
                default:
                    {
                        Engine.Draw(menuBackground, 0, 0);

                        Engine.DrawText("Press 1, 2 or 3 to select difficulty", 100, 420, 0, 0, 255, fuente);
                        Engine.DrawText("Actual difficulty: " + difficulty, 100, 470, 255, 85, 51, fuente);

                        Engine.DrawText("Press C for Classic mode or I for Infinite mode", 100, 570, 0, 0, 0, fuente);
                        Engine.DrawText("Actual game mode: " + (gameMode == 1 ? "Classic" : "Infinite"), 100, 640, 66, 255, 51, fuente);

                        Engine.DrawText("Press T to activate / deactivate tutorial", 100, 740, 66, 255, 51, fuente);
                        Engine.DrawText("Tutorial: " + (tutorial ? "On" : "Off"), 100, 810, 0, 0, 0, fuente);


                        Engine.DrawText("Press space to start ", 100, 870, 250, 185, 15, fuente);
                        break;
                    }
            }

            Engine.Show();
        }

        private static void RenderMeters()
        {
            if (enemyOnTheRight != null)
            {
                meterRCounter = 10;

                Engine.Draw(meterR[meterRCounter], player.position[0] + 15, player.position[1] + 75);
            }
            else
            {
                if (meterRCounter > 0)
                {
                    meterRCounter--;
                }
                Engine.Draw(meterR[meterRCounter], player.position[0] + 15, player.position[1] + 75);

            }

            if (enemyOnTheLeft != null)
            {
                meterLCounter = 10;
                Engine.Draw(meterL[meterLCounter], player.position[0] - 160 /*meter widh*/ - 15, player.position[1] + 75);

            }
            else
            {
                if (meterLCounter > 0)
                {
                    meterLCounter--;
                }

                Engine.Draw(meterL[meterLCounter], player.position[0] - 160 /*meter widh*/ - 15, player.position[1] + 75);
            }

            Engine.DrawText("HP ", 2, 10, 255, 0, 0, fuente);
            Engine.Draw(meterLives[player.lives], 50, 10);

        }

        private static void DrawTutorialMessages()
        {
            if (tutorialStep == 0)
            {

                Engine.DrawText("These bars represent your attack range", player.position[0] - 300, player.position[1] - 30, 250, 185, 15, fuente);
                Engine.DrawText("|", player.position[0] - 115, player.position[1] + 10, 250, 185, 15, fuente);
                Engine.DrawText("v", player.position[0] - 120, player.position[1] + 40, 250, 185, 15, fuente);
                Engine.DrawText("|", player.position[0] + 92, player.position[1] + 10, 250, 185, 15, fuente);
                Engine.DrawText("v", player.position[0] + 87, player.position[1] + 40, 250, 185, 15, fuente);

                Engine.DrawText("Press SPACE BAR to continue", player.position[0] - 200, player.position[1] - 200, 250, 185, 15, fuente);
            }

            if (tutorialStep == 1)
            {
                Engine.DrawText("When the bars light up", player.position[0] - 200, player.position[1] - 60, 250, 185, 15, fuente);
                Engine.DrawText("the enemies are in range to attack", player.position[0] - 300, player.position[1] - 30, 250, 185, 15, fuente);
                Engine.DrawText("|", player.position[0] - 115, player.position[1] + 10, 250, 185, 15, fuente);
                Engine.DrawText("v", player.position[0] - 120, player.position[1] + 40, 250, 185, 15, fuente);
                Engine.DrawText("|", player.position[0] + 92, player.position[1] + 10, 250, 185, 15, fuente);
                Engine.DrawText("v", player.position[0] + 87, player.position[1] + 40, 250, 185, 15, fuente);
                Engine.DrawText("Press SPACE BAR to continue", player.position[0] - 200, player.position[1] - 200, 250, 185, 15, fuente);

            }

            if (tutorialStep == 2)
            {
                Engine.DrawText("Everytime you miss or attack before enemy is in range", player.position[0] - 440, player.position[1] - 180, 250, 185, 15, fuente);
                Engine.DrawText("You get a longer cooldown on your attack", player.position[0] - 320, player.position[1] - 130, 250, 185, 15, fuente);
                Engine.DrawText("So attack only when enemy is in range", player.position[0] - 270, player.position[1] - 80, 250, 185, 15, fuente);
                Engine.DrawText("Or you could be hit by enemy", player.position[0] - 220, player.position[1] - 30, 250, 185, 15, fuente);
                Engine.DrawText("|", player.position[0] - 115, player.position[1] + 10, 250, 185, 15, fuente);
                Engine.DrawText("v", player.position[0] - 120, player.position[1] + 40, 250, 185, 15, fuente);
                Engine.DrawText("|", player.position[0] + 92, player.position[1] + 10, 250, 185, 15, fuente);
                Engine.DrawText("v", player.position[0] + 87, player.position[1] + 40, 250, 185, 15, fuente);
                Engine.DrawText("Press SPACE BAR to continue", player.position[0] - 200, player.position[1] - 250, 250, 185, 15, fuente);
            }
            if (tutorialStep == 3)
            {
                Engine.DrawText("^", player.position[0] + 46, 50, 250, 185, 15, fuente);
                Engine.DrawText("|", player.position[0] + 50, 70, 250, 185, 15, fuente);
                if (gameMode == 1)
                {
                    Engine.DrawText("Your objective is to kill " + enemiesObjective + " enemies", player.position[0] - 250, 120, 250, 185, 15, fuente);
                }
                else
                {
                    Engine.DrawText("In Infinite you try to survive as long as possible", player.position[0] - 400, 120, 250, 185, 15, fuente);
                }
                Engine.DrawText("Press SPACE BAR to continue", player.position[0] - 200, player.position[1] - 200, 250, 185, 15, fuente);
            }

            if (tutorialStep == 4)
            {
                if (enemyOnTheLeft != null)
                {
                    Engine.DrawText("Press 'A', <- or Left click to attack enemy on the left", player.position[0] - 350, player.position[1] - 50, 250, 185, 15, fuente);
                }
                if (enemyOnTheRight != null)
                {
                    Engine.DrawText("Press 'D', -> or Right click to attack enemy on the right", player.position[0] - 350, player.position[1] - 50, 250, 185, 15, fuente);

                }
            }
        }

    }
}