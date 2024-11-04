﻿using MyGame.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using Tao.Sdl;

namespace MyGame
{

    class Program
    {

        static Image background = Engine.LoadImage("assets/backs/" + (new Random().Next(9) + 1) + ".jpg");
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
        static int playerHittedTime = 200;

        static bool tutorial = true;

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
            Animation[] attkEffects = new Animation[] { new Animation("assets/effects/atk1", "atk", 8, false, 2000, new int[] { -140, 70 }, 10) };
            player = new Character(new int[] { 512, 415 }, 4, "assets/spearguy/default/idleR1.png", 0, new Animation("assets/spearguy/default", "idle", 4, true, 2000, new int[] { 0, 0 }, 0), 
                new Animation("assets/spearguy/attack", "spear", 8, false, 1000, new int[] { 0, 0 }, 0), 150, attkEffects, new Animation("assets/spearguy/hit", "hit", 4, false, 1000, new int[] { 0, 0 }, 0), new Animation("assets/spearguy/death", "death", 6, false, 1000, new int[] { 0, 0 }, 0));
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
        }

        private static void PunchRight()
        {
            player.attack(true);

            if (enemyOnTheRight != null)
            {
                enemyOnTheRight.lives--;
                if (enemyOnTheRight.lives <= 0)
                {
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

            enemyOnTheLeft = enemies.Where(enemy => player.position[0]  - distanceToPunch * 2 /*(WHY????)*/ < enemy.position[0] && player.position[0] > enemy.position[0]).FirstOrDefault();
            enemyOnTheRight = enemies.Where(enemy => player.position[0]  + distanceToPunch + player.spriteMid > enemy.position[0] && player.position[0] + player.spriteMid < enemy.position[0]).FirstOrDefault();

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
                        enemy.position[0] -= enemy.speed / 4;
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
            if (spawnTime > 0)
                spawnTime -= delay;
            if (cooldown > 0)
                cooldown -= delay;
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
                enemies.Add(new Character(new int[] { xPos, player.position[1] }, 1, "assets/personaje-anim-1.gif", enemySpeed, new Animation("assets", "slime", 1, true, 500, new int[] { 0, 0 }, 0), new Animation("assets", "slime", 1, true, 500, new int[] { 0, 0 }, 0), 64, new Animation[] { },
                    new Animation("assets", "slime", 1, true, 500, new int[] { 0, 0 }, 0), new Animation("assets", "slime", 1, true, 500, new int[] { 0, 0 }, 0)));
            }
        }

        static void EnemyHitsPlayer() {
            Character enemyHitsPlayer = enemies.Where(enemy => enemy.position[0] < player.position[0] + 15 && enemy.position[0] > player.position[0] - 15).FirstOrDefault();
            if (enemyHitsPlayer != null)
            {
                player.GetHit();
                enemies.Remove(enemyHitsPlayer);
                playerHitted = true;
                playerHittedTime = 500;
                /*TODO Actually hacerlo retroceder*/
                //TODO AND STOP MOVEMENT?
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
                        //DEBUG SPAWNTIME
                        Engine.DrawText("" + spawnTime, 0, 100, 0, 255, 0, fuente);
                        //DEBUG COOLDOWN
                        Engine.DrawText("cool " + cooldown, player.position[0], player.position[1] - 50, 0, 255, 0, fuente);

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
                Engine.Draw(meterR[meterRCounter], player.position[0] + 15 , player.position[1] + 90);
            }
            else
            {
                meterRCounter = 0;
                Engine.Draw(meterR[0], player.position[0] + 15, player.position[1] + 90);

            }

            if (enemyOnTheLeft != null)
            {
                if (meterLCounter < 10)
                {
                    meterLCounter++;
                }
                Engine.Draw(meterL[meterLCounter], player.position[0] - 128 /*meter widh*/ - 15, player.position[1] + 90);
            }
            else
            {
                meterLCounter = 0;
                Engine.Draw(meterL[0], player.position[0] - 128 /*meter widh*/ - 15, player.position[1] + 90);
            }

            Engine.Draw(meterLives[player.lives], 0,0);

        }
    }
}