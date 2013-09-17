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
    public class Menu
    {
        public Texture2D imageTexture;
        public Rectangle imageRectangle;
        public Color color;
        public Menu(Game game, Texture2D _texture, Color _color)
        {
            imageTexture = _texture;
            color = _color;
        }
    }
}
