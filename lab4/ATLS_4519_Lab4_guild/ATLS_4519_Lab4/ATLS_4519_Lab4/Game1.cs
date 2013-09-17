using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace ATLS_4519_Lab4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 

    //define enumerated type for menu (code adapted from "Pong Clone in XNA for Windows" -Ross's Blog)
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
       
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MenuScreen menu;
        Texture2D menuTxt;
        
        Texture2D menuBack;
        SpriteFont Font1;
        //Vector2 FontPos;
        Rectangle sourceRectpleft;
        Rectangle sourceRectpright;
        Rectangle sourceRectBall;
        Texture2D win1Scr;
        int currentItem;
        int kbinterval=0;
        Song bgm;
        newGameSingle newGameSingle1;
        newGameMulti newGameMulti1;
        //bool songStart = false;

        //sound effects
    
        SoundEffect p1;
        SoundEffect p2;

        SoundEffect ballhit;

      
      
        Boolean done1 = false;  // if true, we will display the victory/consolation message
        Boolean done2 = false;
     

        public string victory;  // used to hold the congratulations/better luck next time message

        //anamation for sprites
        /*float timer = 0f;
        float interval = 100f;
        int currentFrameBall = 0;
        int currentFramepleft = 0;
        int currentFramepright = 0; 

        int pleftWidth = 48;
        int pleftHeight = 124;

        int prightWidth = 48;
        int prightHeight = 124;

        int ballspriteHeight = 40;
        int ballspriteWidth = 40; 
        */
          

       //record mouse states 
        //MouseState mouse;
        //MouseState prevMouse;
        

        //record mouse position?
        //Vector2 mousePosition;

       
        //  Paddles 
        Paddle computer_paddle;
        Paddle player_paddle;
        Win1Screen winScrPl;
        // Ball
        Ball ball;
        Texture2D bgi;
 

        //singleplayer/multiplayer button
        //gameMode = true (single player), gameMode = false(multi player) 
        
    
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

          

            //  changing the back buffer size changes the window size (when in windowed mode)
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            currentItem = 1;
            IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
           

            //Load the SoundEffect resources
            // Load the SoundEffect resource
            p1 = Content.Load<SoundEffect>("p1m");
            p2 = Content.Load<SoundEffect>("p2m");
            bgm = Content.Load<Song>("bgm");
            ballhit = Content.Load<SoundEffect>("ball");
            win1Scr=Content.Load<Texture2D>("p1win");
            bgi = Content.Load<Texture2D>("bg-Recovered");
            MediaPlayer.IsRepeating = true;

            menuBack = Content.Load<Texture2D>("title");
            
            menuTxt = Content.Load<Texture2D>("menu1");
            menu = new MenuScreen(this, menuBack, menuTxt);
            
            Components.Add(menu);
            menu.Show();

            winScrPl = new Win1Screen(this, win1Scr, spriteBatch);
            Font1 = Content.Load<SpriteFont>("Courier New");
            sourceRectpleft = new Rectangle(0, 0, 48, 124);
            sourceRectpright = new Rectangle(0, 0, 48, 124);
            sourceRectBall = new Rectangle(0, 0, 40, 40);
            computer_paddle = new Paddle(Content.Load<Texture2D>("p1"), new Vector2(20,384), new Vector2(42, 124),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            player_paddle = new Paddle(Content.Load<Texture2D>("p2"), new Vector2(950,384), new Vector2(42, 124),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            ball = new Ball(Content.Load<Texture2D>("ballsprite"), new Vector2(384, 284), new Vector2(40, 40),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            newGameSingle1 = new newGameSingle(this, computer_paddle, player_paddle, bgm, p1, p2, ballhit, ball, graphics, spriteBatch, bgi);
            newGameMulti1 = new newGameMulti(this, computer_paddle, player_paddle, bgm, p1, p2, ballhit, ball, graphics, spriteBatch, bgi);
            //  set the speed the objects will move
            
            // the ball always starts in the middle and moves toward the player
            ball.Reset();
            computer_paddle.velocity = new Vector2(0, 2);
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //  Free the previously alocated resources
            computer_paddle.texture.Dispose();
            player_paddle.texture.Dispose();
            ball.texture.Dispose();

        }
        void KeyboardHandle()
        {
            KeyboardState kbState = Keyboard.GetState();
            kbinterval++;
            if (kbinterval >= 3)
            {
                kbinterval = 0;
                if (menu.Enabled)
                {
                    if (kbState.IsKeyDown(Keys.Left))
                    {
                        currentItem--;
                        if (currentItem < 1)
                        {
                            currentItem = 3;
                            menu.GetKey(currentItem);
                        }
                        else
                        {
                            menu.GetKey(currentItem);
                        }
                    }
                    if (kbState.IsKeyDown(Keys.Right))
                    {
                        currentItem++;
                        if (currentItem > 3)
                        {
                            currentItem = 1;
                            menu.GetKey(currentItem);
                        }
                        else
                        {
                            menu.GetKey(currentItem);
                        }
                    }
                    if (kbState.IsKeyDown(Keys.Enter))
                    {
                        if (currentItem == 1)
                        {
                            menu.Hide();
                            newGameSingle1.Show();
                        }
                        if (currentItem == 2)
                        {
                            menu.Hide();
                            newGameMulti1.Show();
                        }
                        if (currentItem == 3)
                        {
                            this.Exit();
                        }
                    }
                }
            }
            if (newGameSingle1.Enabled)
            {
                if(kbState.IsKeyDown(Keys.Escape))
                {
                    newGameSingle1.Hide();
                    menu.Show();
                }
            }
            if (newGameMulti1.Enabled)
            {
                if (kbState.IsKeyDown(Keys.Escape))
                {
                    newGameMulti1.Hide();
                    menu.Show();
                }
            }
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

            // this check allows to freeze the game to display the victory/consolation message
            
            KeyboardHandle();
            if (newGameSingle1.Enabled && newGameSingle1.Visible)
            {
                newGameSingle1.Update(gameTime);
            }
            if (newGameMulti1.Enabled && newGameMulti1.Visible)
            {
                newGameMulti1.Update(gameTime);
            }


            if (ball.scorePlayer > ball.scoreFinal)
            {
                
                done1 = true;
                
            }
            else if (ball.scoreComputer > ball.scoreFinal)
            {
                
                done2 = true;
                
            }
                
                if (done1 == false && done2==false)
                {
                    base.Update(gameTime);
                }
        
        }
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the sprites
            spriteBatch.Begin();
            if (newGameSingle1.Visible)
            {
                newGameSingle1.Draw(gameTime);
            }
            if (newGameMulti1.Visible)
            {
                newGameMulti1.Draw(gameTime);
            }
                // Draw running score string
            spriteBatch.DrawString(Font1, "Computer: " + ball.scoreComputer, new Vector2(5, 10), Color.Yellow);
            spriteBatch.DrawString(Font1, "Player: " + ball.scorePlayer,
                new Vector2(graphics.GraphicsDevice.Viewport.Width - Font1.MeasureString("Player: " + ball.scorePlayer).X - 5, 10), Color.Yellow);

            


            if (done1)
            {
                newGameSingle1.Hide();
                newGameMulti1.Hide();
                
                winScrPl.Draw(gameTime);
            }
            else if (done2)
            {
                newGameSingle1.Hide();
                newGameMulti1.Hide();
                
                winScrPl.Draw(gameTime);
            }
            //Draw the other sprites
            //spriteBatch.Draw(ball.texture, ball.position, sourceRectBall, Color.White);
            //spriteBatch.Draw(player_paddle.texture, player_paddle.position, sourceRectpleft, Color.White);
            //spriteBatch.Draw(computer_paddle.texture, computer_paddle.position, sourceRectpright, Color.White);
            //computer_paddle.Draw(spriteBatch);
            //player_paddle.Draw(spriteBatch);
            //ball.Draw(spriteBatch;
            base.Draw(gameTime);
            if (menu.Enabled)
            {
                menu.Draw(gameTime);
            }
            spriteBatch.End();
            
            /*if (done == false)
            {
                base.Draw(gameTime);
            }*/

        }
    }
}
