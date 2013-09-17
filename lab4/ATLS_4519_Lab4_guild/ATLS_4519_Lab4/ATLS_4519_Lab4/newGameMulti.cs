using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ATLS_4519_Lab4
{
    public class newGameMulti : Screen
    {
        // Move the ball and computer paddle
        // All the Move calls do is adjust velocity; position will be changed after
        // velocity adjustments.
        //  Paddles 
        GraphicsDeviceManager graphics;
        Paddle computer_paddle;
        Paddle player_paddle;
        Rectangle sourceRectpleft;
        Rectangle sourceRectpright;
        Rectangle sourceRectBall;
        bool songStart = false;
        Song bgm;
        Texture2D bgi;
        SpriteBatch spriteBatch;
        //sound effects

        MouseState mouse;
        MouseState prevMouse;
        SoundEffect p1;
        SoundEffect p2;

        SoundEffect ballhit;
        float timer = 0f;
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

        // Ball
        Ball ball;
        public newGameMulti(Game game, Paddle _computer_paddle, Paddle _player_paddle, Song _bgm, SoundEffect _p1, SoundEffect _p2, SoundEffect _ballhit, Ball _ball, GraphicsDeviceManager _graphics, SpriteBatch _spriteBatch, Texture2D _bgi)
            : base(game)
        {
            computer_paddle = _computer_paddle;
            player_paddle = _player_paddle;
            //sourceRectpleft = _sourceRectpleft;
            //sourceRectpright = _sourceRectpright;
            bgm = _bgm;
            p1 = _p1;
            p2 = _p2;
            ballhit = _ballhit;
            ball = _ball;
            graphics = _graphics;
            spriteBatch = _spriteBatch;
            bgi = _bgi;

        }

        public override void Update(GameTime gameTime)
        {
            if (!songStart)
            {
                MediaPlayer.Play(bgm);
                songStart = true;
            }
            ball.Move(player_paddle, computer_paddle);
            //computer_paddle.Move(ball);

            if (ball.collision_occured_right)
            {
                p1.Play();
                ball.collision_occured_right = false;
                //ball.playit = true;
            }
            if (ball.collision_occured_left)
            {
                p2.Play();
                ball.collision_occured_left = false;
                //ball.playit = true;
            }
            if (ball.collision_occured && ball.playit)
            {
                ballhit.Play();
                ball.playit = false;
            }

            //allow mouse to select botton for multiplayer/single player 
            mouse = Mouse.GetState();


            // mousePosition = new Vector2 (mouse.X, mouse.Y);





            prevMouse = mouse;

            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrameBall++;
                currentFramepright++;
                currentFramepleft++;
                sourceRectpleft = new Rectangle(currentFramepleft * pleftWidth, 0, pleftWidth, pleftHeight);
                sourceRectpright = new Rectangle(currentFramepright * prightWidth, 0, prightWidth, prightHeight);
                sourceRectBall = new Rectangle(currentFrameBall * ballspriteWidth, 0, ballspriteWidth, ballspriteHeight);
                timer = 0f;
            }

            if (currentFrameBall == 5)
            {
                currentFrameBall = 0;
            }
            if (currentFramepleft == 2)
            {
                currentFramepleft = 0;
            }
            if (currentFramepright == 2)
            {
                currentFramepright = 0;
            }


            //TODO: play sounds for paddle miss and kill shots
            // This will require ball and paddle to tell us when to do this

            // Now adjust postion
            ball.position += ball.velocity;

            //computer_paddle.position += computer_paddle.velocity;
            //computer_paddle.Move(ball);

            // Move the player paddle
            // Change the player paddle position using the left thumbstick, mouse or keyboard 

            //Thumbstick
            Vector2 LeftThumb = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
            player_paddle.position += new Vector2(LeftThumb.X, -LeftThumb.Y) * 5;

            //  Change the player paddle position using the keyboard
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Up))
                if (player_paddle.position.Y > 0)  // don't run off the edge
                    player_paddle.position += new Vector2(0, -8);
            if (keyboardState.IsKeyDown(Keys.Down))
                if (player_paddle.position.Y < (graphics.PreferredBackBufferHeight - player_paddle.size.Y)) // don't run off the edge
                    player_paddle.position += new Vector2(0, 8);
            if (keyboardState.IsKeyDown(Keys.W))
                if (computer_paddle.position.Y > 0)  // don't run off the edge
                    computer_paddle.position += new Vector2(0, -8);
            if (keyboardState.IsKeyDown(Keys.S))
                if (computer_paddle.position.Y < (graphics.PreferredBackBufferHeight - player_paddle.size.Y)) // don't run off the edge
                    computer_paddle.position += new Vector2(0, 8);


            //  Make the player paddle follow the mouse, but only in Y

            //if (player_paddle.position.Y < Mouse.GetState().Y)
            //    player_paddle.position += new Vector2(0, 5);
            //if (player_paddle.position.Y > Mouse.GetState().Y)
            //    player_paddle.position += new Vector2(0, -5);

        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(bgi, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(ball.texture, ball.position, sourceRectBall, Color.White);
            spriteBatch.Draw(player_paddle.texture, player_paddle.position, sourceRectpleft, Color.White);
            spriteBatch.Draw(computer_paddle.texture, computer_paddle.position, sourceRectpright, Color.White);
            base.Draw(gameTime);
        }
    }
    }

