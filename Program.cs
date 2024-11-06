using MyGame.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using Tao.Sdl;

namespace MyGame
{

    class Program
    {

        static Image background = Engine.LoadImage("assets/backs/" + (new Random().Next(7) + 1) + ".jpg");
        static Image[] meterL;
        static int meterLCounter = 0;
        static Image[] meterR;
        static int meterRCounter = 0;
        static Image[] meterLives;

        public static int delay = 10;
        static int gameState = 0;

        static int gravity = delay * 2;
        static int jumpSpeedInitial = delay * 2;
        static int jumpSpeedForce = 2;
        static int jumpSpeed = 0;

        static int spawnTime = 2000;
        static int enemySpawnTime = 500;
        static Random enemyRandomDirection = new Random();
        static Random enemyRandomTime = new Random();

        static Character player;
        static List<Character> enemies = new List<Character>();
        static List<Character> deadEnemies = new List<Character>();
        static List<Character> inactiveEnemies = new List<Character>();

        static int distanceToPunch = 80;

        static int cooldownTime = 100;
        static int cooldownMissTime = 300;
        static int cooldown = 0;

        static Font fuente;
        static int mouseX = 0;
        static int mouseY = 0;

        static bool pressed = false;

        static int idleCounter = 0;
        static Character enemyOnTheLeft = null;
        static Character enemyOnTheRight = null;

        static bool playerHitted = false;
        static int playerHittedTime = 700;

        static bool tutorial = true;
        static Random enemyRandom = new Random();

        static void Main(string[] args)
        {
            Engine.Initialize();

            initializeMeters(out meterL, out meterR, out meterLives);
            initializePlayer();

            fuente = Engine.LoadFont("assets/arial.ttf", 40);

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
            Animation[] attkEffects = new Animation[] { new Animation(AssetsUtils.assets.playerAttkREffects[0], AssetsUtils.assets.playerAttkLEffects[0], false, 2000, new int[] { -140, 70 }, 10, false) };
            player = new Character(new int[] { 512, 425 }, 4, 0, 150, new Animation(AssetsUtils.assets.playerIdleRImages, AssetsUtils.assets.playerIdleLImages, true, 2000, new int[] { 0, 0 }, 0, false), 
                new Animation(AssetsUtils.assets.playerAtkRImages, AssetsUtils.assets.playerAtkLImages, false, 1000, new int[] { 0, 0 }, 0, false), attkEffects, new Animation(AssetsUtils.assets.playerHitRImages, AssetsUtils.assets.playerHitLImages, false, 2000, new int[] { 0, 0 }, 0, false), new Animation(AssetsUtils.assets.playerDeathRImages, AssetsUtils.assets.playerDeathLImages, false, 1000, new int[] { 0, 0 }, 0, false));
        }

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

            if (Engine.KeyPress(Engine.KEY_SPACE) && !pressed)
            {
                if (gameState == 0)
                {
                    gameState = 1;
                }
                if (gameState == 2)
                {
                    ResetGame();
                    gameState = 0;
                }
                pressed = true;
            }

            if (cooldown <= 0)
            {
                if ((Engine.KeyPress(Engine.KEY_LEFT) || Engine.KeyPress(Engine.KEY_A) || Engine.MouseClick(Engine.MOUSE_LEFT, out mouseX, out mouseY)) && !pressed)
                {
                    PunchLeft();
                    pressed = true;
                }

                if ((Engine.KeyPress(Engine.KEY_RIGHT) || Engine.KeyPress(Engine.KEY_D) || Engine.MouseClick(Engine.MOUSE_RIGHT, out mouseX, out mouseY)) && !pressed)
                {
                    PunchRight();
                    pressed = true;
                }
                if(!(Engine.KeyPress(Engine.KEY_LEFT) || Engine.KeyPress(Engine.KEY_A) || Engine.KeyPress(Engine.KEY_RIGHT) || Engine.KeyPress(Engine.KEY_D) || Engine.KeyPress(Engine.KEY_SPACE)))
                {
                    pressed = false;
                }
            }

            if (Engine.KeyPress(Engine.KEY_DOWN) || Engine.KeyPress(Engine.KEY_S))
            {
            }

            if (Engine.KeyPress(Engine.KEY_ESC))
            {
                Environment.Exit(0);
            }
        }

        private static void ResetGame()
        {
            enemies = new List<Character>();
            cooldown = 0;
            player.lives = 4;
            tutorial = true;
        }

        private static void PunchRight()
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
                }
                cooldown = cooldownTime;
                if (tutorial)
                {
                    tutorial = false;
                }
            }
            else
            {
                cooldown = cooldownMissTime;
            }
        }

        private static void PunchLeft()
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
                }
                cooldown = cooldownTime;

            }
            else
            {
                cooldown = cooldownMissTime;
            }
        }

        static void Update()
        {
            inactiveEnemies.ForEach(enemy => deadEnemies.Remove(enemy));

            enemyOnTheLeft = enemies.Where(enemy => player.position[0]  - distanceToPunch * 2 /*(WHY????)*/ < enemy.position[0] && player.position[0] > enemy.position[0]).FirstOrDefault();
            enemyOnTheRight = enemies.Where(enemy => player.position[0]  + distanceToPunch + player.spriteMid > enemy.position[0] && player.position[0] < enemy.position[0]).FirstOrDefault();

            SpawnEnemy();

            MoveEnemies();
            
            EnemyHitsPlayer();
           
        }

        private static void MoveEnemies()
        {
            //Moving enemies
            if (player.lives > 0 && (!tutorial || (enemyOnTheLeft == null && enemyOnTheRight == null)))
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

        private static void SpawnEnemy()
        {
            if (cooldown > 0)
                cooldown -= delay;
            if (playerHitted) 
            {
                return;
            }
            if (spawnTime > 0)
                spawnTime -= delay;

            /* random appearance logic condition here {}*/
            if (spawnTime <= 0 && player.lives > 0 && (!tutorial || (enemyOnTheLeft == null && enemyOnTheRight == null)))
            {
                spawnTime = enemySpawnTime + (enemyRandomTime.Next(2) == 0 ? enemyRandomTime.Next(150) : (enemyRandomTime.Next(350) * -1));
                int xPos = enemyRandomDirection.Next(2) == 0 ? (0 - 100) : (1024 + 100);
                if (tutorial && enemies.Count < 2)
                {
                    if(enemies.Count < 1)
                    {
                        xPos = 0 - 100;
                    } else
                    {
                        xPos = 1024 + 100;
                    }
                }
                int enemySpeed = delay / 2 * (xPos <= 0 ? 1 : -1); //Enemy speed direction
                int enemyRandomAsset = enemyRandom.Next(2);
                enemies.Add(new Character(new int[] { xPos, player.position[1] }, 1, enemySpeed, 150, new Animation(AssetsUtils.assets.enemiesWalkRImages[enemyRandomAsset], AssetsUtils.assets.enemiesWalkLImages[enemyRandomAsset], true, 3000, new int[] { 0, 0 }, 0, enemySpeed > 0), new Animation(AssetsUtils.assets.enemiesAtkRImages[enemyRandomAsset], AssetsUtils.assets.enemiesAtkLImages[enemyRandomAsset], false, 700, new int[] { 0, 0 }, 0, enemySpeed > 0), new Animation[] { },
                    new Animation(AssetsUtils.assets.enemiesIdleRImages[enemyRandomAsset], AssetsUtils.assets.enemiesIdleLImages[enemyRandomAsset], false, 500, new int[] { 0, 0 }, 0, enemySpeed > 0), new Animation(AssetsUtils.assets.enemiesDeathRImages[enemyRandomAsset], AssetsUtils.assets.enemiesDeathLImages[enemyRandomAsset], false, 5000, new int[] { 0, 0 }, 0, enemySpeed > 0)));
            }
        }

        static void EnemyHitsPlayer() {
            Character enemyHitsPlayer = enemies.Where(enemy => enemy.position[0] < player.position[0] + 35 && enemy.position[0] > player.position[0] - 35).FirstOrDefault();
            if (enemyHitsPlayer != null && !playerHitted)
            {
                player.GetHit();
                enemyHitsPlayer.attack(enemyHitsPlayer.speed > 0);
                playerHitted = true;
                playerHittedTime = 700;
            }

            if (playerHitted)
            {
                playerHittedTime -= delay;
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
                        Engine.Draw(background, 0, 0);
                        //Draw player
                        bool playerRender = player.Render();
                        //Player dies
                        if (player.lives <= 0 && playerRender)
                        {
                            //TODO STOP MOVEMENT
                            gameState = 2;
                        }

                        enemies.ForEach(enemy =>
                        {
                            //Draw enemies
                            enemy.Render();
                            //DEBUG POSITION
                            Engine.DrawText("" + enemy.position[0], enemy.position[0], enemy.position[1] - 80, 0, 255, 0, fuente);
                        });
                        deadEnemies.ForEach(enemy =>
                        {
                            //Draw dead enemies
                            if (enemy.Render()) {
                                inactiveEnemies.Add(enemy);
                            }
                            
                            //DEBUG POSITION
                            Engine.DrawText("" + enemy.position[0], enemy.position[0], enemy.position[1] - 80, 0, 255, 0, fuente);
                        });
                        //DEBUG SPAWNTIME
                        Engine.DrawText("" + spawnTime, 0, 100, 0, 255, 0, fuente);
                        //DEBUG COOLDOWN
                        Engine.DrawText("" + player.position[0], player.position[0], player.position[1] - 50, 0, 255, 0, fuente);

                        RenderMeters();

                        //DEBUG
                        Engine.DrawText("|", player.position[0], player.position[1] , 0, 255, 0, fuente);

                        if (tutorial)
                        {
                            if (enemyOnTheLeft != null)
                            {
                                Engine.DrawText("Press A, <- or Left click to hit enemy", player.position[0], player.position[1] - 150, 0, 255, 0, fuente);
                            }
                            if (enemyOnTheRight != null)
                            {
                                Engine.DrawText("Press D, -> or Right click to hit enemy", player.position[0], player.position[1] - 150, 0, 255, 0, fuente);
                            }
                        }

                        break;
                    }
                case 2:
                    {
                        Engine.DrawText("Game over", 200, 400, 0, 255, 0, fuente);
                        break;
                    }
                default:
                    {
                        Engine.DrawText("Press space to start ", 200, 400, 0, 255, 0, fuente);
                        break;
                    }
            }

            Engine.Show();
        }

        private static void RenderMeters()
        {
            if (enemyOnTheRight != null)
            {
                if (meterRCounter < 10)
                {
                    meterRCounter++;
                }
                Engine.Draw(meterR[meterRCounter], player.position[0] + 15 , player.position[1] + 75);
            }
            else
            {
                meterRCounter = 0;
                Engine.Draw(meterR[0], player.position[0] + 15, player.position[1] + 75);

            }

            if (enemyOnTheLeft != null)
            {
                if (meterLCounter < 10)
                {
                    meterLCounter++;
                }
                Engine.Draw(meterL[meterLCounter], player.position[0] - 128 /*meter widh*/ - 15, player.position[1] + 75);
            }
            else
            {
                meterLCounter = 0;
                Engine.Draw(meterL[0], player.position[0] - 128 /*meter widh*/ - 15, player.position[1] + 75);
            }

            Engine.Draw(meterLives[player.lives], 0,0);

        }
    }
}