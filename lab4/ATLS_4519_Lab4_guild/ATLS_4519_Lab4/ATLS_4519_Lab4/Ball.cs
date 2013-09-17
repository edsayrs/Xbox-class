using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;   //   for Texture2D
using Microsoft.Xna.Framework;  //  for Vector2

namespace ATLS_4519_Lab4
{
    public class Ball
    {
        //  Ball texture 
        public Texture2D texture { get; set; } //  ball texture, read-only property
        public Vector2 position { get; set; }  //  ball position on screen
        public Vector2 size { get; set; }      //  ball size in pixels
        public Vector2 velocity { get; set; }  //  ball velocity
        private Vector2 screenSize { get; set; } //  screen size

        public Vector2 center { get { return position + (size / 2); } } //  ball center
        public float radius { get { return size.X / 2; } } //  ball radius

        // Create a random number generator
        Random rnd = new Random();

        public Boolean collision_occured = false;  // keeps track of whether the ball hit a paddle
        public Boolean playit = true;// used to make sure that we only play the sound once each collision
        public Boolean collision_occured_left = false;
        public Boolean collision_occured_right = false;
        public int scorePlayer = 0; //players score
        public int scoreComputer = 0; //computer's score
        public int scoreFinal = 5; // end the game when either player has this many points plus 1

        //constructor
        public Ball(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int ScreenWidth, int ScreenHeight)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = new Vector2(ScreenWidth, ScreenHeight);
        }


        //collision with paddle?  could have combined these, but this is easier reading
        public bool Player_Collision(Paddle paddle)
        {

            //player
            if ((this.position.X + this.size.X >= paddle.position.X) && // right side
                (this.position.Y + this.size.Y >= paddle.position.Y) && //top boundary
                (this.position.Y < paddle.position.Y + paddle.size.Y)) //bottom boundary
                return true;
            else
                return false;
        }

        public bool Computer_Collision(Paddle paddle)
        {
            //computer
            if ((this.position.X <= paddle.position.X + paddle.size.X) &&  // left side
                    (this.position.Y + this.size.Y >= paddle.position.Y) && // top boundary
                    (this.position.Y <= paddle.position.Y + paddle.size.Y)) // bottom boundary
                return true;
            else
                return false;
        }

        public bool Corner_Collision(Paddle paddle)
        {
            // check if the ball hit the corner of a paddle in the kill shot zone
            // assumes we have a collision, so this function should only be called if that is true

            if (((this.position.Y + this.size.Y) <= (paddle.position.Y + Paddle.PADDLE_KILL_SHOT_FRACTION))    // upper corner
               || ((this.position.Y) >= (paddle.position.Y + paddle.size.Y - Paddle.PADDLE_KILL_SHOT_FRACTION)))  // lower corner
                return true;
            else
                return false;
        }


        public void Reset()
        {
            // minus 10 points for using embedded constants; fix this later

            // put a 32 x 32 ball in the middle of a 800 x 600 frame
            position = new Vector2(384, 284);

            //start the ball toward the player, but not always in same direction or with same speed
            velocity = new Vector2((int)rnd.Next(5, 7), (int)rnd.Next(-2, 3));

            // fix it if our random y velocity is horizontal (boring)
            if (velocity.Y == 0) velocity = new Vector2(velocity.X, (int)rnd.Next(-3, -1));
        }


        public void Move(Paddle player, Paddle computer)
        {
            //  Balls have more complex movement than paddles.  If they hit top or bottom, invert Y velocity.
            //  If they hit a paddle, change velocity depending upon where on the paddle we hit
            //  If we the ball misses the paddle and falls off the right or left edge, increase the score of
            //  the other side, decrease this side's number of lives (if implemented), wait a second or two,
            //  put the ball back in the center, and give a random direction.
            //  We might also want (someday) to gradually increase velocity when the player hits the ball back.

            // reset collision_occured when we cross centerline
            // this avoids the odd case where we collide with the paddle while we are leaving the scene
            if ((this.position.X > 350) && (this.position.X < 450)) this.collision_occured = false;

            // check to see if we hit a paddle
            if (Player_Collision(player) && this.collision_occured == false)
            {
                collision_occured = true;
                playit = true;
                // We hit a paddle; where did we hit?
                // If normal, just invert velocity as appropriate, and leave with departure angle equal to attack angle
                // If corner, hit a kill shot
                if (Corner_Collision(player))
                {
                    // kill shot
                    velocity = new Vector2((-this.velocity.X * 2), this.velocity.Y + (int)rnd.Next(-1, 2));
                }
                else
                {
                    // normal ball return; just invert X velocity; play with velocity just a bit

                    velocity = new Vector2(-this.velocity.X, (int)rnd.Next(((int)this.velocity.Y - 1), ((int)this.velocity.Y + 2)));
                }
            }

            else if (Computer_Collision(computer) && this.collision_occured == false)
            {
                collision_occured = true;// We hit a paddle; where did we hit?
                playit = true;
                // If normal, just invert velocity as appropriate, and leave with departure angle equal to attack angle
                // If corner, hit a kill shot
                if (Corner_Collision(computer))
                {
                    // kill shot
                    velocity = new Vector2((-this.velocity.X * 2), this.velocity.Y);
                }
                else
                {
                    // normal ball return; just invert X velocity
                    velocity = new Vector2(-this.velocity.X, (int)rnd.Next(((int)this.velocity.Y - 1), ((int)this.velocity.Y + 2)));
                }
            }

            //we might be at an edge, do the right thing if needed

            //  Check right boundary to see if we missed the paddle
            else if (this.position.X + this.size.X + this.velocity.X > this.screenSize.X)
            {
                // Give the other guy a point
                scoreComputer += 1;
                collision_occured_right = true;
                // Play the paddle miss sound
                //TODO
                // Go again
                Reset();
                // 
            }

            //  Check left boundary to see if we missed the paddle
            else if (this.position.X + this.velocity.X < 0)
            {
                // Give the other guy a point
                scorePlayer += 1;
                collision_occured_left = true;

                // Play the paddle miss sound
                //TODO
                // Go again
                Reset();

            }

            //  Check bottom boundary
            else if (this.position.Y + this.size.Y + this.velocity.Y > this.screenSize.Y)
            {
                this.velocity = new Vector2(this.velocity.X, -this.velocity.Y);
            }

            // Check top boundary
            else if (this.position.Y + this.velocity.Y < 0)
            {
                this.velocity = new Vector2(this.velocity.X, -this.velocity.Y);
            };

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(texture,position, Color.White);
        }
    }
}
