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
    public class Screen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public SpriteBatch sprBatch;
        public Screen(Game game)
            : base(game)
        {
            Visible = false;
            Enabled = false;
            sprBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
        }
        public bool getEnabled()
        {
            return Enabled;
        }
        public void Show()
        {
            Visible = true;
            Enabled = true;
        }
        public void Hide()
        {
            Visible = false;
            Enabled = false;
        }
    }
}
