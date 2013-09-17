using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace ATLS_4519_Lab4
{
    class Win1Screen:Screen
    {
        SpriteBatch spriteBatch;
        Texture2D win1Scr;
        public Win1Screen(Game game, Texture2D _win1Scr, SpriteBatch _spriteBatch):base(game)
        {
            spriteBatch = _spriteBatch;
            win1Scr = _win1Scr;
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(win1Scr, new Vector2(0,0), Color.White);
        }
    }
}
