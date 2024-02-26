using Blish_HUD.Content;
using Blish_HUD;
using Glide;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework.Graphics;
using BoonsUp.Utils;
using Gw2Sharp.Models;
using BoonsUp.Services;
using System.Collections.Generic;

namespace BoonsUp.Controls;

internal class BoonMask : Control
{
    private Rectangle _bounds = new Rectangle();
    private List<Rectangle> _boonCoords = new List<Rectangle>();

    public BoonMask()
    {
        _bounds = BoonCoordinatesService.OnScreenBoonBounds();
        _boonCoords = BoonCoordinatesService.GetBoonCoordinates();
        Size = new Point(_bounds.Width,_bounds.Height);
        Location = new Point(_bounds.X, _bounds.Y);
    }


    public override void DoUpdate(GameTime gameTime)
    {
        Align();

        GameService.Graphics.SpriteScreen.BackgroundColor = Color.Transparent;
    }


    protected void Align()
    {
        var scale = GameService.Graphics.UIScaleMultiplier;
        var _origBounds = BoonCoordinatesService.OnScreenBoonBounds();
        _bounds = _origBounds;// MultiplyByScale(_origBounds,scale);



        _boonCoords = BoonCoordinatesService.GetBoonCoordinates();
        var newList = new List<Rectangle>();
        _boonCoords.ForEach(b => newList.Add(MultiplyByScale(b, scale)));

        _boonCoords = newList;

        Size = new Point(_bounds.Width, _bounds.Height);
        Location = new Point(_origBounds.X, _origBounds.Y);
        //var spriteScreenSize = GameService.Graphics.SpriteScreen.Size;
        //Location = new Microsoft.Xna.Framework.Point(spriteScreenSize.X / 2 + _magicOffset.X, spriteScreenSize.Y + _magicOffset.Y);
        //Size = new Point(_boonSize * _boonCols, _boonSize * _boonRows);
    }
    private Rectangle MultiplyByScale(Rectangle rect, float scale)
    {
        rect.X = (int)(rect.X / scale);
        rect.Y = (int)(rect.Y / scale);
        rect.Width = (int)(rect.Width / scale);
        rect.Height = (int)(rect.Height / scale);
        return rect;
    }

    protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
    {
        //Background
        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, bounds, null, Color.White  * 0.2f 
            
            );
        /*var currentX = bounds.Left;
        for(var i = 0; i <= _boonCols; i++)
        {
            
            drawBox(spriteBatch, new Rectangle(currentX, bounds.Top+_boonSize, _boonSize, _boonSize));
            currentX += _boonSize + _boonPadX;
            currentX -= (i % 2 == 0 ? 0 : 1);
            currentX += (i % 8 != 0 ? 0 : 1);
        }*/
        foreach( var region in _boonCoords ) { 
            drawBox(spriteBatch, region);
        }

    }

    private void drawBox(SpriteBatch spriteBatch, Rectangle box)
    {

        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(box.Left, box.Top, box.Width, 1), Rectangle.Empty, Color.Blue);
        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(box.Left, box.Bottom, box.Width, 1), Rectangle.Empty, Color.Blue);
        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(box.Left, box.Top, 1, box.Height), Rectangle.Empty, Color.Blue);
        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(box.Right, box.Top, 1, box.Height), Rectangle.Empty, Color.Blue);

    }

}
