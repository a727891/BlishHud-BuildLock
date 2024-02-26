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
using System.Drawing.Imaging;

namespace BoonsUp.Controls;

internal class ImageCapture : Control
{
    private Rectangle _bounds = new Rectangle();
    private List<Rectangle> _boonCoords = new List<Rectangle>();
    private String _path = "";
    private String _filePrefix = "unknown";
    BoonTextureCaptureService textures = new BoonTextureCaptureService();

    private bool _isCapturing = false;

    public ImageCapture()
    {

        //_path = Service.DirectoriesManager.GetFullDirectoryPath($"{Module.DIRECTORY_PATH}/images");
        _path = Service.DirectoriesManager.GetFullDirectoryPath(Module.DIRECTORY_PATH);
        //_bounds = BoonCoordinatesService.OnScreenBoonBounds();
        _bounds = new Rectangle(0, 0, 100, 50);
        //_boonCoords = BoonCoordinatesService.GetBoonCoordinates();
        Size = new Point(_bounds.Width,_bounds.Height);
        Location = new Point(_bounds.X, _bounds.Y);

        textures.TexturesUpdated += Textures_TexturesUpdated;
    }

    public bool ToggleCapture()
    {
        var prof = GameService.Gw2Mumble.PlayerCharacter.Profession;
        var spec = GameService.Gw2Mumble.PlayerCharacter.Specialization;
        _filePrefix = $"\\images\\{prof.ToString()}\\{prof}_{spec}";
        System.IO.Directory.CreateDirectory($"{_path}\\images\\{prof.ToString()}");
        return _isCapturing = !_isCapturing;
    }
    
    private void Textures_TexturesUpdated(object sender, bool e)
    {
        textures.GetClippedImage().Save($"{_path}{_filePrefix}_{DateTime.Now.ToFileTimeUtc()}.png", ImageFormat.Png);
    }

    public void Dispose()
    {
        textures.TexturesUpdated -= Textures_TexturesUpdated;
    }


    double timer = 0;

    public override void DoUpdate(GameTime gameTime)
    {
        if (!_isCapturing) return;

        if (!GameService.Gw2Mumble.PlayerCharacter.IsInCombat) return;

        timer += gameTime.ElapsedGameTime.TotalMilliseconds;
        if(timer > 5000)
        {
            Capture();
            timer -= 5000;
            //Align();
        }

        //GameService.Graphics.SpriteScreen.BackgroundColor = Color.Transparent;
    }

    private void Capture()
    {
        textures.TestCapture();
    }


    protected void Align()
    {
        //var scale = GameService.Graphics.UIScaleMultiplier;
        var _origBounds = BoonCoordinatesService.OnScreenBoonBounds();
        _bounds = _origBounds;// MultiplyByScale(_origBounds,scale);



        //_boonCoords = BoonCoordinatesService.GetBoonCoordinates();
        //var newList = new List<Rectangle>();
        //_boonCoords.ForEach(b => newList.Add(MultiplyByScale(b, scale)));

        //_boonCoords = newList;

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

        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, bounds, null, _isCapturing?Color.Red:Color.White );


        //spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, bounds, null, Color.White * 0.2f);
        //drawBox(spriteBatch, bounds);
        /*var currentX = bounds.Left;
        for(var i = 0; i <= _boonCols; i++)
        {
            
            drawBox(spriteBatch, new Rectangle(currentX, bounds.Top+_boonSize, _boonSize, _boonSize));
            currentX += _boonSize + _boonPadX;
            currentX -= (i % 2 == 0 ? 0 : 1);
            currentX += (i % 8 != 0 ? 0 : 1);
        }*/
        /*foreach( var region in _boonCoords ) { 
            drawBox(spriteBatch, region);
        }*/

    }

    private void drawBox(SpriteBatch spriteBatch, Rectangle box)
    {

        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(box.Left, box.Top, box.Width, 1), Rectangle.Empty, Color.Blue);
        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(box.Left, box.Bottom, box.Width, 1), Rectangle.Empty, Color.Blue);
        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(box.Left, box.Top, 1, box.Height), Rectangle.Empty, Color.Blue);
        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(box.Right, box.Top, 1, box.Height), Rectangle.Empty, Color.Blue);

    }

}
