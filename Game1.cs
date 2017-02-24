using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace IWKS_3400_Lab4
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        clsSprite mySpriteBall;    // currently ball32
        clsSprite playerPaddle;   // currently set to pong_paddle
        clsSprite enemyPaddle;   // also set to pong_paddle
        clsSprite playerSmallPaddle;
        clsSprite playerBigPaddle;

        //// Part of the Enum method
        //Texture2D mControllerDetectScreenBakcground;
        //Texture2D mTitleScreenBackground;
        //Texture2D mPauseMenu;

        enum ScreenState
        {
            ControllerDetect,
            Title,
            Play,
            Pause,
            NewRound,
            EndGame
        }

        //Current Screen State
        ScreenState mCurrentScreen;

        SpriteBatch spriteBatch;    /* group of sprites (with same settings) */


        //        MainMenu main = new MainMenu(); 

        SoundEffect soundEffect;    // add a sound effect resource

        int screenWidth = 1280; // Screen Width in pixels
        int screenHeight = 720;    //Screen Height in pixels
        int midSprite = 32;    // The middle of the sprite (This value is arbitrary but depends on clsSprite sizes)
        float gameVelocity = 5;  // The velocity of the ball after a collision with a paddle. Also the speed of the player
        char paddleChoice;  // Selection without state change
        float paddleVelocity = 5f;  //for normal paddle
        float bigVelocity = 2f;   //for big paddle
        float smallVelocity = 10f;  //for small paddle

        SpriteFont gameFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);    /* handles the configuration and management of the graphics device */

            // changing back buffer size changes the window size (in windowed mode) (values are arbitrary within a range)
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;


            Content.RootDirectory = "Content";   /* naming our root dirctory to the name "Content" */
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            gameFont = Content.Load<SpriteFont>("SpriteFont1");
            

            //Initialize the current screen state to the screen we want to display first
            mCurrentScreen = ScreenState.ControllerDetect;

            // Load (2) 2D texture sprites <ball.png>
            mySpriteBall = new clsSprite(Content.Load<Texture2D>("ball32"),
                new Vector2(screenWidth - 72f, 0f), new Vector2(32f, 32f),
                screenWidth, screenHeight);//  (0,0) is top left of screen, (64, 64) is size of sprite 

            playerPaddle = new clsSprite(Content.Load<Texture2D>("pong_paddle"),     /* (218,118) represents the top right of screen,  64^2 is size */
               new Vector2(40f, 118f), new Vector2(10f, 64f),
               screenWidth, screenHeight);

            enemyPaddle = new clsSprite(Content.Load<Texture2D>("pong_paddle"),
                new Vector2(screenWidth - 40f, 118f), new Vector2(10f, 64f),
                screenWidth, screenHeight);

            playerBigPaddle = new clsSprite(Content.Load<Texture2D>("big_paddle"),     /* (218,118) represents the top right of screen,  64^2 is size */
                   new Vector2(40f, 118f), new Vector2(20f, 128f),
                  screenWidth, screenHeight);
            midSprite = 64;
            paddleVelocity = bigVelocity;
            soundEffect = Content.Load<SoundEffect>("LASER");  //but 1 octave lower

            playerSmallPaddle = new clsSprite(Content.Load<Texture2D>("small_paddle"),     /* (218,118) represents the top right of screen,  64^2 is size */
                 new Vector2(40f, 118f), new Vector2(5f, 32f),
                screenWidth, screenHeight);
            midSprite = 16;
            paddleVelocity = smallVelocity;
            soundEffect = Content.Load<SoundEffect>("LASER");  //but 1 octave higher

            // Load Sound Effect Resource
            soundEffect = Content.Load<SoundEffect>("LASER");
            mySpriteBall.velocity = new Vector2(-5, 5);   /* setting initial x and y velocity (values arbitrary) */

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            // Free previously allocated resources
            mySpriteBall.texture.Dispose();
            playerPaddle.texture.Dispose();
            enemyPaddle.texture.Dispose();
            playerBigPaddle.texture.Dispose();
            playerSmallPaddle.texture.Dispose();
            spriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            switch (mCurrentScreen)
            {
                case ScreenState.ControllerDetect:
                    {
                        UpdateControllerDeleterScreen();
                        break;
                    }
                case ScreenState.Title:
                    {
                        UpdateTitleScreen();
                        break;
                    }
                case ScreenState.Play:
                    {
                        //Move the sprite(s)
                        mySpriteBall.Move();

                        if (paddleChoice == 'A')
                        {
                            playerSmallPaddle.position = new Vector2(-1000f, 0f);       //pushes these other two paddles off the screen so they don't interfere with the game
                            playerPaddle.position = new Vector2(-1000f, 0f);
                            playerBigPaddle.PlayerMoveRestrition();
                            keyBoardControls(playerBigPaddle);
                        }
                        else if (paddleChoice == 'C')
                        {
                            playerBigPaddle.position = new Vector2(-1000f, 0f);
                            playerPaddle.position = new Vector2(-1000f, 0f);
                            playerSmallPaddle.PlayerMoveRestrition();
                            keyBoardControls(playerSmallPaddle);
                        }
                        else if (paddleChoice == 'B')
                        {
                            playerSmallPaddle.position = new Vector2(-1000f, -1000f);
                            playerBigPaddle.position = new Vector2(-1000f, -1000f);
                            playerPaddle.PlayerMoveRestrition();
                            keyBoardControls(playerPaddle);
                        }
                        enemyPaddle.ChaseBallSprite(mySpriteBall);
                        // playerPaddle.ChaseBallSprite(mySpriteBall);    // This if for CPU vs CPU

                        keyBoardControls(playerPaddle);
                        // mouseControls(playerPaddle);   // This is for mouse control options


                        if (mySpriteBall.Collides(enemyPaddle))    /* calling our circle collide method for the two sprites */
                        {
                            rhsCPUCollision(mySpriteBall, enemyPaddle);
                            gameVelocity += 0.2f;
                            enemyPaddle.cpuChaseSpeed += 0.15f;

                            soundEffect.Play();
                        }

                        if (mySpriteBall.Collides(playerPaddle))
                        {
                            lhsPlayerCollision(mySpriteBall, playerPaddle); // Player vs CPU
                                                                            // lhsCPUCollision(mySpriteBall, playerPaddle);   // this is for CPU vs CPU
                            gameVelocity += 0.2f;
                            enemyPaddle.cpuChaseSpeed += 0.15f;

                            soundEffect.Play();
                        }

                        if (mySpriteBall.Collides(playerBigPaddle))
                        {
                            lhsPlayerCollision(mySpriteBall, playerBigPaddle); // Player vs CPU
                            bigVelocity += 0.1f;                                                 // lhsCPUCollision(mySpriteBall, playerPaddle);   // this is for CPU vs CPU
                            gameVelocity += 0.2f;
                            enemyPaddle.cpuChaseSpeed += 0.15f;

                            soundEffect.Play(1.0f, -1.0f, 0f);         //make octave lower?
                        }

                        if (mySpriteBall.Collides(playerSmallPaddle))
                        {
                            lhsPlayerCollision(mySpriteBall, playerSmallPaddle); // Player vs CPU
                            smallVelocity += 0.4f;                                                // lhsCPUCollision(mySpriteBall, playerPaddle);   // this is for CPU vs CPU
                            gameVelocity += 0.2f;
                            enemyPaddle.cpuChaseSpeed += 0.15f;

                            soundEffect.Play(1.0f, 1.0f, 0f);
                        }

                        if (mySpriteBall.velocity.Y == 0)
                        {
                            UpdateRound();
                        }

                        int x = 3;
                        if (mySpriteBall.PlayerScore == x || mySpriteBall.CPUScore == x)
                        {
                            mCurrentScreen = ScreenState.EndGame;
                        }

                        // Check if the game is to be paused.
                        UpdatePlayScreen();
                        break;
                    }
                case ScreenState.Pause:
                    {
                        UpdateTitleScreen();
                        break;
                    }
                case ScreenState.NewRound:
                    {
                        UpdateTitleScreen();
                        break;
                    }
                case ScreenState.EndGame:
                    {
                        UpdateGame();
                        break;
                    }
            }



            base.Update(gameTime);    // updates graphic renders 
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);    /* sets the background fill color (im going against the grain with BurlyWood) */

            // TODO: Add your drawing code here

            //Draw the sprite using Alpha Blend, which uses transperency information if available
            // In 4.0, this behavior is the default; in XNA 3.1, it is not
            spriteBatch.Begin();
            //  main.Draw(spriteBatch);
            switch (mCurrentScreen)
            {
                case ScreenState.ControllerDetect:
                    {
                        DrawControllerDetectScreen();
                        break;
                    }
                case ScreenState.Title:
                    {
                        DrawTitleScreen();
                        break;
                    }
                case ScreenState.Play:
                    {
                        DrawGame();
                        break;
                    }
                case ScreenState.Pause:
                    {
                        DrawGame();
                        break;
                    }
                case ScreenState.NewRound:
                    {
                        DrawNewRound();
                        break;
                    }
                case ScreenState.EndGame:
                    {
                        DrawEndGame();
                        break;
                    }
            }


            spriteBatch.End();

            base.Draw(gameTime);    /* draws the game recursively */
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Methods for Game1. Must be Private!! 

        // Pre: Game has started.  
        // Post: The method runs recursively as a part of the update method. Player alters Sprite Velocity
        private void keyBoardControls(clsSprite playerSprite)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (paddleChoice == 'A')
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    playerSprite.position += new Vector2(0, -bigVelocity);
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    playerSprite.position += new Vector2(0, bigVelocity);
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    playerSprite.position += new Vector2(0, 0);    // No x movement in pong
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    playerSprite.position += new Vector2(0, 0);    // No x movement in pong
                }
            }
            else if (paddleChoice == 'B')
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    playerSprite.position += new Vector2(0, -gameVelocity);
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    playerSprite.position += new Vector2(0, gameVelocity);
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    playerSprite.position += new Vector2(0, 0);    // No x movement in pong
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    playerSprite.position += new Vector2(0, 0);    // No x movement in pong
                }
            }
            else if (paddleChoice == 'C')
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    playerSprite.position += new Vector2(0, -smallVelocity);
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    playerSprite.position += new Vector2(0, smallVelocity);
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    playerSprite.position += new Vector2(0, 0);    // No x movement in pong
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    playerSprite.position += new Vector2(0, 0);    // No x movement in pong
                }
            }

        }

        // Pre: Game has started and sprites are in motion.
        // Post: Method runs recursively in the update method. Sprite follows mouse Y position.
        private void mouseControls(clsSprite playerSprite)
        {
            if (playerPaddle.position.X < Mouse.GetState().X)
            {
                playerPaddle.position += new Vector2(0, 0);
            }
            if (playerPaddle.position.X > Mouse.GetState().X)
            {
                playerPaddle.position += new Vector2(0, 0);
            }
            if (playerPaddle.position.Y < Mouse.GetState().Y)
            {
                playerPaddle.position += new Vector2(0, gameVelocity);   // No x direction movement allowed
            }
            if (playerPaddle.position.Y > Mouse.GetState().Y)
            {
                playerPaddle.position += new Vector2(0, -gameVelocity);    // No X direction movement allowed
            }
        }

        //Pre: A collision has occured on the right hand side of the screen. 
        //Post: The velocity of the pong is adjusted based on where it hit the CPU paddle
        private void rhsCPUCollision(clsSprite lhs, clsSprite rhs)
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);

            lhs.position.X -= 1;  // prevents sprites getting stuck together
            if ((lhs.position.Y + midSprite) > (rhs.position.Y + midSprite))
            {
                lhs.velocity = new Vector2(-gameVelocity, gameVelocity);
            }
            if ((lhs.position.Y + midSprite) <= (rhs.position.Y + midSprite))
            {
                lhs.velocity = new Vector2(-gameVelocity, -gameVelocity);
            }
        }

        //Pre: A collision has occured on the left hand side of the screen. 
        //Post: The velocity of the pong is adjusted based on where it hit the CPU paddle
        private void lhsCPUCollision(clsSprite lhs, clsSprite rhs)
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);

            lhs.position.X += 1;  // prevents sprites getting stuck together
            if ((lhs.position.Y + lhs.halfSizeY) > (rhs.position.Y + rhs.halfSizeY))
            {
                lhs.velocity = new Vector2(gameVelocity, gameVelocity);
            }
            if ((lhs.position.Y + lhs.halfSizeY) <= (rhs.position.Y + rhs.halfSizeY))
            {
                lhs.velocity = new Vector2(gameVelocity, -gameVelocity);
            }
        }

        //Pre: A collision has occured between the pongBall and the left hand side player sprite
        //Post: modifies the velocity and direction based on player movements (if player isn't moving, position determines trajectory)
        private void lhsPlayerCollision(clsSprite lhs, clsSprite rhs)
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            KeyboardState keyboardState = Keyboard.GetState();

            lhs.position.X += 1;    // This helps prevent sticking collision (ball direction ->)
            if (keyboardState.IsKeyDown(Keys.Up) || ((lhs.position.Y + lhs.halfSizeY) < (rhs.position.Y + rhs.halfSizeY)))  // if we are moving up, so will the ball
            {
                lhs.velocity = new Vector2(gameVelocity, -gameVelocity);
            }
            if (keyboardState.IsKeyDown(Keys.Down) || ((lhs.position.Y + lhs.halfSizeY) > (rhs.position.Y + rhs.halfSizeY)))   // if we are moving down, so will the ball
            {
                lhs.velocity = new Vector2(gameVelocity, gameVelocity);
            }
        }

        //Pre: 
        //Post:p
        private void UpdateControllerDeleterScreen()
        {
            //Poll all the gamepads (and the keyboard) to check to see
            //which controller will be the player one controller
            for (int i = 0; i < 2; i++)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
                {
                    mCurrentScreen = ScreenState.Title;
                    return;
                }
            }
        }

        //Pre:
        //Post:
        private void UpdateTitleScreen()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.B) == true)
            {
                mCurrentScreen = ScreenState.ControllerDetect;
                return;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) == true)
            {

                mCurrentScreen = ScreenState.Play;
                return;
            }
        }

        private void UpdateGame()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) == true)
            {
                mySpriteBall.PlayerScore = 0;
                mySpriteBall.CPUScore = 0;
                gameVelocity = 5;
                enemyPaddle.cpuChaseSpeed = gameVelocity;

                if (mySpriteBall.PlayerScore == 0 && mySpriteBall.CPUScore == 0)
                {
                    mCurrentScreen = ScreenState.NewRound;
                    return;
                }
            }
        }

        //Pre:
        //Post:
        private void UpdatePlayScreen()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P) == true)
            {
                mCurrentScreen = ScreenState.Pause;
            }
        }

        private void UpdateRound()
        {
            mCurrentScreen = ScreenState.NewRound;
        }


        //Pre:  Controller Detect Screen is initialized. Method is called in Draw() and Update()
        //Post: Controller Screen is the screen state
        private void DrawControllerDetectScreen()
        {
            spriteBatch.DrawString(gameFont, "Press [ENTER] to Continue... ", new Vector2(screenWidth / 2 - 140f, screenHeight / 2), Color.White);
        }

        //Pre: Background title initialized. Method is callied in Draw() and Update()
        //Post: Title Screen is the screen state
        private void DrawTitleScreen()
        {
            spriteBatch.DrawString(gameFont, "V^2 PONG 1.0.0", new Vector2(screenWidth / 2 - 140f, 10), Color.Yellow);
            spriteBatch.DrawString(gameFont, "Choose your Paddle", new Vector2(screenWidth / 2 - 140f, screenHeight / 2 - 100f), Color.AntiqueWhite);
            spriteBatch.DrawString(gameFont, "A: Big Paddle", new Vector2(screenWidth / 2 - 140f, screenHeight / 2), Color.Gold);
            spriteBatch.DrawString(gameFont, "B: Normal Paddle", new Vector2(screenWidth / 2 - 140f, screenHeight / 2 + 20f), Color.GreenYellow);
            spriteBatch.DrawString(gameFont, "C: Tiny Paddle", new Vector2(screenWidth / 2 - 140f, screenHeight / 2 + 40f), Color.Red);
            if (Keyboard.GetState().IsKeyDown(Keys.A) == true)              //not sure how to make this turn these messages into an actual message on screen, draw another screenstate with title and whichver paddle was chose?
            {
                paddleChoice = 'A';
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.B) == true)
            {
                paddleChoice = 'B';
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.C) == true)
            {
                paddleChoice = 'C';
            }
            if (paddleChoice == 'A')
            {
                spriteBatch.DrawString(gameFont, "Big Paddle Chosen!", new Vector2(screenWidth / 2 - 140f, screenHeight / 2 - 30f), Color.White);
            }
            else if (paddleChoice == 'B')
            {
                spriteBatch.DrawString(gameFont, "Normal Paddle Chosen!", new Vector2(screenWidth / 2 - 140f, screenHeight / 2 - 30f), Color.White);
            }
            else if (paddleChoice == 'C')
            {
                spriteBatch.DrawString(gameFont, "Tiny Paddle Chosen!", new Vector2(screenWidth / 2 - 140f, screenHeight / 2 - 30f), Color.White);
            }

            spriteBatch.DrawString(gameFont, "Press [SPACE] to Play... ", new Vector2(screenWidth / 2 - 140f, screenHeight / 2 + 100f), Color.Green);
        }


        private void DrawPauseMenu()
        {
            spriteBatch.DrawString(gameFont, "Pause[P] Play[Space] ", new Vector2(screenWidth / 2 - 140f, 10), Color.Red);
        }

        //Pre:
        //Post: Draws the in game sprites (players and objects) 
        private void DrawGame()
        {
            spriteBatch.DrawString(gameFont, "Player: " + mySpriteBall.PlayerScore, new Vector2(5, 10), Color.Yellow);
            spriteBatch.DrawString(gameFont, "Computer: " + mySpriteBall.CPUScore, new Vector2(screenWidth - 140f, 10), Color.Yellow);
            spriteBatch.DrawString(gameFont, "Pause[P] Play[Space] ", new Vector2(screenWidth / 2 - 140f, 10), Color.Red);
            spriteBatch.DrawString(gameFont, "Current Speed: " + mySpriteBall.velocity.X + "," + mySpriteBall.velocity.Y,
                new Vector2(screenWidth / 2 - 140f, screenHeight - 40f), Color.Blue);
            mySpriteBall.Draw(spriteBatch);
            if (paddleChoice == 'A')
            {
                playerBigPaddle.Draw(spriteBatch);
            }
            else if (paddleChoice == 'C')
            {
                playerSmallPaddle.Draw(spriteBatch);
            }
            else
            {
                playerPaddle.Draw(spriteBatch);
            }
            enemyPaddle.Draw(spriteBatch);
        }



        //Pre:
        //Post: Draws a newRound Screen. Player position is "random" to increase challenge
        private void DrawNewRound()
        {
            var rnd = new Random(DateTime.Now.Second);
            int ticks = rnd.Next(64, 720 - 64);
            mySpriteBall.position = new Vector2(screenWidth - 72f, screenHeight / 2);
            if (paddleChoice == 'A')
            {
                playerBigPaddle.position = new Vector2(40f, ticks);
            }
            else if (paddleChoice == 'C')
            {
                playerSmallPaddle.position = new Vector2(40f, ticks);
            }
            else if (paddleChoice == 'B')
            {
                playerPaddle.position = new Vector2(40f, ticks);
            }
            enemyPaddle.position = new Vector2(screenWidth - 40f, screenHeight / 2);
            DrawGame();
        }
        //Pre: A score Limit has been reached (A winner has won) -Happens in Update()
        //Post: Draws an endgame screen
        private void DrawEndGame()
        {
            spriteBatch.DrawString(gameFont, "Player: " + mySpriteBall.PlayerScore, new Vector2(5, 10), Color.White);
            spriteBatch.DrawString(gameFont, "Computer: " + mySpriteBall.CPUScore, new Vector2(screenWidth - 140f, 10), Color.White);
            spriteBatch.DrawString(gameFont, "[Space] to go again", new Vector2(screenWidth / 2 - 140f, 10), Color.Red);

            int playerS = mySpriteBall.PlayerScore;
            int cpuS = mySpriteBall.CPUScore;

            if (playerS > cpuS)
            {
                spriteBatch.DrawString(gameFont, " YOU WIN! amazing...", new Vector2(screenWidth / 2 - 140f, screenHeight / 2), Color.Yellow);
            }
            if (playerS < cpuS)
            {
                spriteBatch.DrawString(gameFont, "YOU LOSE! SKYNET IS HAPPENING!!", new Vector2(screenWidth / 2 - 140f, screenHeight / 2), Color.Red);
            }
            

        }
    }
}