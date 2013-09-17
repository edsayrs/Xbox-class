using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;   //   for Texture2D
using Microsoft.Xna.Framework;  //  for Vector2

namespace ATLS_4519_Lab4
{
    class Paddle
    {
        //  Paddle texture 
        public Texture2D texture { get; set; } //  paddle texture, read-only property
        public Vector2 position { get; set; }  //  paddle position on screen
        public Vector2 size { get; set; }      //  paddle size in pixels
        public Vector2 velocity { get; set; }  //  paddle velocity
        private Vector2 screenSize { get; set; } //  screen size
        public const int PADDLE_KILL_SHOT_FRACTION = 8;   // the portion of the paddle (in pixels) on each end that makes a kill shot


        public Paddle(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int ScreenWidth, int ScreenHeight)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = new Vector2(ScreenWidth, ScreenHeight);
        }


        public void Move(Ball ball)
        {
            //  Paddles can only move in the Y direction, and cannot leave the screen
            //  If paddle moves to screen edge, invert velocity

            //  check top and bottom boundaries
            if ((position.Y + size.Y + velocity.Y > screenSize.Y) || (position.Y + velocity.Y < 0))
            {
                velocity *= -1;
            }

            // now see if need to change direction to keep paddle Y near ball Y
            // This simple test accomplishes our computer player "AI"
            else if ((position.Y > ball.position.Y) && (velocity.Y > 0) ||
                    (position.Y < ball.position.Y) && (velocity.Y < 0))
            {
                velocity *= -1;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}




