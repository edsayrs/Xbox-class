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
    class MenuScreen: Screen
    {
        Rectangle menuR;
        Texture2D menuTxt;
        Texture2D menuBack;
        int rWidth = 265;
        int rHeight = 145;
        public MenuScreen(Game game, Texture2D _menuBack, Texture2D _menuTxt)
            : base(game)
        {
            menuBack = _menuBack;
            menuR = new Rectangle(0, 0, rWidth, rHeight);
            menuTxt = _menuTxt;
            
           
        }

        public void GetKey(int i)
        {
            if (i == 1)
            {
                menuR = new Rectangle((i-1)*rWidth, 0, rWidth, rHeight);
            }
            if (i == 2)
            {
                menuR = new Rectangle((i-1)*rWidth, 0, rWidth, rHeight);
            }
            if (i == 3)
            {
                menuR = new Rectangle((i-1)*rWidth, 0, rWidth, rHeight);
            }
           
        }
         public override void Draw(GameTime gameTime)
        {
            
            sprBatch.Draw(menuBack, new Vector2(0,0), Color.White);
            sprBatch.Draw(menuTxt, new Vector2(759,0), menuR, Color.White);
            
            base.Draw(gameTime);
            
        }
        }
       }
;
