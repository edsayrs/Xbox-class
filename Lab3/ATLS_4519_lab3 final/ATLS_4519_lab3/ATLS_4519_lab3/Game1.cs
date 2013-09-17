using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace ATLS_4519_lab3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        // Sprite objects
        clsSprite mySprite1;
        clsSprite mySprite2;
        // SpriteBatch which will draw (render) the sprite
        SpriteBatch spriteBatch;

        //Create a SoundEffect resource
        SoundEffect soundEffect; 

        //create AudioEngine resource from xact library 
        AudioEngine audioEngine;
        //create  SoundBank resource from xact library
        SoundBank soundBank;
        //create WaveBank resource from xact library 
        WaveBank waveBank;
        //create Cue resource from xact library 
        Cue cue;

        //create a streaming wavebank and cue resource from xact library 
        WaveBank streamingWaveBank;
        Cue musicCue;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // changing the back buffer size changes the window size (when in windowed mode)
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load files built from XACT project
            audioEngine = new AudioEngine("Content\\Lab3Sounds.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Sound Bank.xsb");
            // Load streaming wave bank
            streamingWaveBank = new WaveBank(audioEngine, "Content\\Music.xwb", 0, 4);
            // The audio engine must be updated before the streaming cue is ready
            audioEngine.Update();
            // Get cue for streaming music
            musicCue = soundBank.GetCue("Music");
            // Start the background music
            musicCue.Play();

            // Load a 2D texture sprite
            mySprite1 = new clsSprite(Content.Load<Texture2D>("ball"), new Vector2(0f, 0f), new Vector2(64f, 64f),
            graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            mySprite2 = new clsSprite(Content.Load<Texture2D>("ball"), new Vector2(218f, 118f), new Vector2(64f, 64f),
            graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            // Create a SpriteBatch to render the sprite
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            //load the SoundEffect resource
            soundEffect = Content.Load<SoundEffect>("chord"); 
            // set the velocity of the two sprites will move
            mySprite1.velocity = new Vector2(5, 5);
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Free the previously alocated resources
            mySprite1.texture.Dispose();
            mySprite2.texture.Dispose();
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
// Move the sprites
if(mySprite1.Move())
{
    soundEffect.Play(0.5f,0.0f,0.0f);
}
//  Change the sprite 2 position using the left thumbstick 
//Vector2 LeftThumb = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
//mySprite2.position += new Vector2(LeftThumb.X, -LeftThumb.Y) * 5;

//  Change the sprite 2 position using the keyboard
KeyboardState keyboardState = Keyboard.GetState();
if (keyboardState.IsKeyDown(Keys.Up))
    mySprite2.position += new Vector2(0, -5);
if (keyboardState.IsKeyDown(Keys.Down))
    mySprite2.position += new Vector2(0, 5);
if (keyboardState.IsKeyDown(Keys.Left))
    mySprite2.position += new Vector2(-5, 0);
if (keyboardState.IsKeyDown(Keys.Right))
    mySprite2.position += new Vector2(5, 0);

  /*Make sprite 2 follow the mouse 
if (mySprite2.position.X < Mouse.GetState().X)
  mySprite2.position += new Vector2(5, 0);
if (mySprite2.position.X > Mouse.GetState().X)
    mySprite2.position += new Vector2(-5, 0);
    if (mySprite2.position.Y < Mouse.GetState().Y)
   mySprite2.position += new Vector2(0, 5);
if (mySprite2.position.Y > Mouse.GetState().Y)
   mySprite2.position += new Vector2(0, -5);
   */

   if (mySprite1.CircleCollides(mySprite2))
            {
                mySprite1.velocity *= -1;
                GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                //soundEffect1.Play(1.0f,-0.5f,-1.0f);
                // Get an instance of the cue from the XACT project
                cue = soundBank.GetCue("Explosion");
                cue.Play();
            }
else
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            // Update the AudioEngine
   audioEngine.Update();
base.Update(gameTime);
}
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            // Draw the sprite using Alpha Blend, which uses transparency information if available
            // In 4.0, this behavior is the default; in XNA 3.1, it is not
            spriteBatch.Begin();
            mySprite1.Draw(spriteBatch);
            mySprite2.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

