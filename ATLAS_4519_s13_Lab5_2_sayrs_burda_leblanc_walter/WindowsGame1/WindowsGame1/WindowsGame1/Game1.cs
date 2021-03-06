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
using Microsoft.Xna.Framework.Storage;

namespace ATLS_4519_Lab5
{
    /// <summary>
    /// This is the main type for the game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        Texture2D backgroundTexture; //The texture that will define the game background.
        Texture2D state1;
        Texture2D state2;
        Texture2D state3;
        Texture2D state4;
        Texture2D mouseBody;
        Texture2D mouseheadnshoulders;
        Texture2D owl1;
        Texture2D owl2;
        Texture2D owl3;
        Texture2D arrow;

        int lives = 3;
        ///<summary>
        ///The Viewport is used to draw on part of the screen. It should be set before any
        ///geometry is drawn, so that the Viewport parameters will take effect.
        ///To draw multiple views within a scene, repeat seeting Viewport and draw a geometry sequence for each view. 
        /// </summary>
        Rectangle viewportRect;
        SpriteBatch spriteBatch; //Gets or sets an object that uniquely identifies this sprite batch. 

        //Game compontent variables.
        GameObject cannon; //The cannon will be controlled by human input.
        const int maxCannonBalls = 3; //Maximum number of cannonballs the cannon is able to shoot at a time.
        GameObject[] cannonBalls;

        //Sets up user input. 
        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
        KeyboardState previousKeyboardState = Keyboard.GetState();

        float gravity = 0;
        //Enemy variables.
        const int maxEnemies = 3; //Maximum number of enemies rendered on the screen at a time. 
        const float maxEnemyHeight = 0.1f;
        const float minEnemyHeight = 0.5f;
        const float maxEnemyVelocity = 5.0f;
        const float minEnemyVelocity = 1.0f;
        Random random = new Random(); // create a source of random integers 
        GameObject[] enemies;

        //Score keeping variables. 
        int score;
        SpriteFont font;
        Vector2 scoreDrawPoint = new Vector2(0.1f, 0.1f);

        // Sound Effects
        SoundEffect bowshot;
        SoundEffect birdhit;
        Song bgmusicrough;
        SoundEffect gameover;
        SoundEffect hit1;
        SoundEffect hit2;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 1000;
            graphics.ApplyChanges();
         
            base.Initialize();

        }

        /// <summary>
        /// Load your graphics content
        /// </summary>
        protected override void LoadContent()
        {
            //Creates a new SpriteBatch, which is used to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            // Load the SoundEffect resource
            bowshot = Content.Load<SoundEffect>("bowshot");
            bgmusicrough  = Content.Load<Song>("bgmusicrough");
            gameover = Content.Load<SoundEffect>("gameover");
            hit1 = Content.Load<SoundEffect>("hit1");
            hit2 = Content.Load<SoundEffect>("hit2");
            birdhit = Content.Load<SoundEffect>("birdhit");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bgmusicrough);


            //Initialize game background.
            backgroundTexture =
                   Content.Load<Texture2D>("background");
            state1 =
                   Content.Load<Texture2D>("state1");
            state2 =
                   Content.Load<Texture2D>("state2");
            state3 =
                   Content.Load<Texture2D>("state3");
            state4 =
                   Content.Load<Texture2D>("state4");
            arrow =
                   Content.Load<Texture2D>("arrow");
            owl1 =
                   Content.Load<Texture2D>("owl1");
            owl2 =
                   Content.Load<Texture2D>("owl2");
            owl3 =
                   Content.Load<Texture2D>("owl3");
            mouseBody =
                   Content.Load<Texture2D>("mousebody");
            mouseheadnshoulders =
                   Content.Load<Texture2D>("mouseheadnshoulders");

            //Initialize the position and texture of the player's cannon. 
            cannon = new GameObject(mouseheadnshoulders);
            cannon.position = new Vector2(668, 253);


            //Initialize an array of new cannonball GameObjects that can be fired by the player.
            cannonBalls = new GameObject[maxCannonBalls];
            for (int i = 0; i < maxCannonBalls; i++)
            {
                //Load player's cannonball sprite.
                cannonBalls[i] = new GameObject(arrow);
            }

            //Initialize enemies.
            enemies = new GameObject[maxEnemies];
            for (int i = 0; i < maxEnemies; i++)
            {
                //Load enemy sprite.
                if (i % 3 == 0)
                {
                    enemies[i] = new GameObject(
                    owl1);
                }
                if (i % 3 == 1)
                {
                    enemies[i] = new GameObject(
                    owl2);
                }
                if (i % 3 == 2)
                {
                    enemies[i] = new GameObject(
                    owl3);
                }
            }

            //Load the font sprite that will be used for Score keeping.
            font = Content.Load<SpriteFont>("Arial");
            
            //Create a Rectangle that represents the full drawable area of the game screen.
            viewportRect = new Rectangle(0, 0,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);

            base.LoadContent();
        }

        
        //This method must be called once per frame.
        public void UpdateCannonBalls()
        {
            //"foreach" is a C# construct that we haven't used before.  Look it up.
            foreach (GameObject ball in cannonBalls)
            {
                if (ball.alive)
                {
                    ///<summary>
                    ///Here we check to see if the cannonball is alive; to do this,
                    ///we move it and do a simple check to determine if cannonball is offscreen.
                    ///If you were to add gravity, it would need to update every frame so that
                    ///the cannonball follows a realistic ballistic projectile path.
                    ///</summary>
                    gravity += 0.0005f;
                    ball.velocity.Y += gravity;
                    ball.position += ball.velocity;
                    if (!viewportRect.Contains(new Point(
                        (int)ball.position.X,
                        (int)ball.position.Y)))
                    {
                        ball.alive = false;
                        gravity = 0;
                        continue; //we're done here.
                    }

                    //Creates an "invisible" rectangle that is the size of the cannonball sprite,
                    //used (later) to perform collision detection. 
                    Rectangle cannonBallRect = new Rectangle(
                        (int)ball.position.X,
                        (int)ball.position.Y,
                        ball.sprite.Width,
                        ball.sprite.Height);

                    //Check to see if the cannonball rectangle intersects with enemy rectangle.
                    foreach (GameObject enemy in enemies)
                    {
                        //Construct rectangle at the coordinates of the enemies on screen.
                        Rectangle enemyRect = new Rectangle(
                            (int)enemy.position.X,
                            (int)enemy.position.Y,
                            enemy.sprite.Width,
                            enemy.sprite.Height);

                        //If collision between cannonball and enemy, then eliminate the alien ship.
                        if (cannonBallRect.Intersects(enemyRect))
                        {
                            ball.alive = false;
                            enemy.alive = false;
                            //make an explosions sound
                            hit1.Play();
                            score += 1;
                            break;
                        }
                    }
                }
            }
        }

        //Creates a method that render and kill enemies. 
        public void UpdateEnemies()
        {
            //634 204
            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {
                    //Check to see if enemy is alive.
                    enemy.position += enemy.velocity;
                    if (enemy.position.X > 700)
                    {
                        
                        lives -= 1;
                        enemy.alive = false;
                    }
                    if (!viewportRect.Contains(new Point(
                        (int)enemy.position.X,
                        (int)enemy.position.Y)))
                    {
                        enemy.alive = false;
                    }
                }
                    
                else
                {
                    enemy.alive = true;
                    enemy.position = new Vector2(
                        viewportRect.Left,
                        MathHelper.Lerp(
                        (float)viewportRect.Height * minEnemyHeight,
                        (float)viewportRect.Height * maxEnemyHeight,
                        (float)random.NextDouble()));
                    enemy.velocity = new Vector2(
                        MathHelper.Lerp(
                        minEnemyVelocity,
                        maxEnemyVelocity,
                        (float)random.NextDouble()), 0);
                }

            }
        }

        //Creates a method that will use the rotation of the cannon to fire a cannonball at the proper velocity.
        public void FireCannonBall()
        {
            foreach (GameObject ball in cannonBalls)
            {
                if (!ball.alive)
                {
                    bowshot.Play();
                    ball.alive = true;
                    ball.position = new Vector2(632,254);//This allows the cannonball to be shot from the middle of the cannon.
                    ball.velocity = new Vector2(
                        (float)Math.Cos(cannon.rotation - MathHelper.Pi),
                        (float)Math.Sin(cannon.rotation -MathHelper.Pi)) * 5.0f;
                    ball.rotation = cannon.rotation;
                    return;
                }
            }
        }

        /// <summary>
        /// Unload any content not managed by the Content Manager
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Checks for user input from Xbox360 controller.
            //Uses controller joysticks to move the cannon. 
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            cannon.rotation += gamePadState.ThumbSticks.Left.X * 0.1f;
            if (gamePadState.Buttons.A == ButtonState.Pressed &&
                previousGamePadState.Buttons.A == ButtonState.Released) //Pressing A fires the cannonball.
            {
                FireCannonBall();
            }
           

#if !XBOX   // This is how to include XBOX-specific or Windows-specific code
            // Checks for user input and computer keyboard.
            // Uses arrow keys to move the cannon.

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                cannon.rotation -= 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                cannon.rotation += 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.Space) &&
                previousKeyboardState.IsKeyUp(Keys.Space)) //Pressing spacebar fires cannonball.
            {
                FireCannonBall();
            }
#endif
            //Determines how far you can rotate the cannon.
            //Clamp restricts a value to be within a specified range (in this case, 0 to 90 degrees).
            cannon.rotation = MathHelper.Clamp(cannon.rotation, -MathHelper.PiOver4, MathHelper.PiOver4);

            //Update our game objects and soun2
            UpdateCannonBalls();
            UpdateEnemies();
            //Add future game logic here. 

            previousGamePadState = gamePadState;
#if !XBOX
            previousKeyboardState = keyboardState;
#endif

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Draw the backgroundTexture sized to the width
            //and height of the screen.
            spriteBatch.Draw(backgroundTexture, viewportRect,
                Color.White);
            switch (lives)
            {
                case 0:
                    {
                        spriteBatch.Draw(state4, viewportRect, Color.White);
                        gameover.Play();
                        break;
                    }
                case 1 :
                {
                    spriteBatch.Draw(state3, viewportRect, Color.White);
                    spriteBatch.Draw(mouseBody, cannon.position - new Vector2(50, 30),
                Color.White);
                    spriteBatch.Draw(cannon.sprite,
                cannon.position,
                null,
                Color.White,
                cannon.rotation,
                cannon.center, 1.0f,
                SpriteEffects.None, 0);
                    
                   
                    break;
                }
                case 2:
                {
                    spriteBatch.Draw(state2, viewportRect, Color.White);
                    spriteBatch.Draw(mouseBody, cannon.position - new Vector2(50, 30),
                Color.White);
                    spriteBatch.Draw(cannon.sprite,
                cannon.position,
                null,
                Color.White,
                cannon.rotation,
                cannon.center, 1.0f,
                SpriteEffects.None, 0);
                    
                  
                    
                    break;
                }
                case 3:
                {
                    spriteBatch.Draw(state1, viewportRect, Color.White);
                    spriteBatch.Draw(mouseBody, cannon.position - new Vector2(50, 30),
                Color.White);
                    spriteBatch.Draw(cannon.sprite,
                cannon.position,
                null,
                Color.White,
                cannon.rotation,
                cannon.center, 1.0f,
                SpriteEffects.None, 0);
                    
                    
                    break;
                }
                default:
                {
                    spriteBatch.Draw(state4, viewportRect, Color.White);
                    break;
                }       

            }

            //Draw player cannonballs only if they are alive.
            foreach (GameObject ball in cannonBalls)
            {
                if (ball.alive)
                {
                    spriteBatch.Draw(ball.sprite,
                        ball.position, Color.Red);
                }
            }

          

            //Draw the alien ships.
            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {
                    spriteBatch.Draw(enemy.sprite,
                        enemy.position, Color.White);
                }
            }

            //Draws the scoreboard.
            spriteBatch.DrawString(font,
                "Score: " + score.ToString(),  //note the use of the ToString function
                //Indicates coordinates on where to draw the score. 
                new Vector2(scoreDrawPoint.X * viewportRect.Width,
                scoreDrawPoint.Y * viewportRect.Height),
                Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
