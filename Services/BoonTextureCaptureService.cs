using Blish_HUD;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using DrawPoint = System.Drawing.Point;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using BoonsUp.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace BoonsUp.Services;

public class BoonTextureCaptureService : IDisposable
{
    private int _trimmedHeight = 3;
    private int _trimmedOffset = 12;
    private int _trimmedOffsetX = 4;
    private Rectangle _trimRegion;

    private List<Texture2D> _boonTextures = new List<Texture2D>();

    private Bitmap _screenCaptureImage = new Bitmap(1, 1);
    private int _boonCount = BoonCoordinatesService.GetTotalBoonCount();
    private Bitmap[] _boonSlots = new Bitmap[BoonCoordinatesService.GetTotalBoonCount()];
    private Bitmap[] _boonSlotsTrimmed = new Bitmap[BoonCoordinatesService.GetTotalBoonCount()];
    private Point _boonSize = BoonCoordinatesService.GetBoonSize();
    private List<Rectangle> _boonCoords = BoonCoordinatesService.GetBoonCoordinates();

    private double _throttleLimit = 5000;
    private double _throttleAccumulator = 0;

    public event EventHandler<bool>? TexturesUpdated;

     

    public BoonTextureCaptureService()
    {
        Init();
    }

    public List<Texture2D> GetBoonTexture2D()
    {
        return _boonTextures;
    }
    public (Bitmap[] textures, Bitmap[] trimmed) GetTextures()
    {
        return (_boonSlots, _boonSlotsTrimmed);
    }
    public Bitmap GetClippedImage()
    {
        return _screenCaptureImage;
    }

    private void Init()
    {
        _trimRegion = new Rectangle(_trimmedOffsetX, _trimmedOffset, _boonSize.X-(2*_trimmedOffsetX), _trimmedHeight);

        using (Blish_HUD.Graphics.GraphicsDeviceContext device = Blish_HUD.GameService.Graphics.LendGraphicsDeviceContext())
        {
            
            for (int i = 0; i < _boonCount; i++)
            {
                _boonSlots[i] = new Bitmap(_boonSize.X, _boonSize.Y);
                //_boonTextures.Add(new Texture2D());
                _boonTextures.Add(new Texture2D(device.GraphicsDevice, _boonSize.X, _boonSize.Y));
                _boonSlotsTrimmed[i] = new Bitmap(_boonSize.X, _trimmedHeight);
            }
        }
    }

    public void DoUpdate(GameTime gameTime)
    {

        _throttleAccumulator += gameTime.ElapsedGameTime.TotalMilliseconds;

        if(_throttleAccumulator >= _throttleLimit)
        {
            CaptureBitmaps();
            _throttleAccumulator = 0;
            TexturesUpdated?.Invoke(this, true);
        }
    }
    public void TestCapture()
    {

        CaptureBitmaps();
        TexturesUpdated?.Invoke(this, true);
    }

    private void CaptureBitmaps()
    {
        Rectangle screenBounds = BoonCoordinatesService.OnScreenBoonBounds();
        if (_screenCaptureImage.Size != screenBounds.ToSystemDrawingSize())
        {
            _screenCaptureImage?.Dispose();
            _screenCaptureImage = new Bitmap(screenBounds.Width, screenBounds.Height);
        }

        CaptureRegionOnScreen(screenBounds, _screenCaptureImage);

        //Generate boonslot textures and trimmed bitmaps
        for (int i = 0; i < _boonCount; i++)
        {
            ClipRegionFromImage(_screenCaptureImage, _boonCoords[i], _boonSlots[i]);
            _boonTextures[i].SetFromBitmap(_boonSlots[i]);
            ClipRegionFromImage(_boonSlots[i], _trimRegion, _boonSlotsTrimmed[i]);
        }
    }


    public void Dispose()
    {
        foreach (var item in _boonSlots)
        {
            item?.Dispose();
        }
        foreach (var item in _boonSlotsTrimmed)
        {
            item?.Dispose();
        }
        _screenCaptureImage?.Dispose();
    }

    private Bitmap ClipRegionFromImage(Bitmap image, Rectangle region) {
        Bitmap bitmap = new(region.Width, region.Height);

        GraphicsUtils.CopyRegionIntoImage(image, region.ToSystemDrawingRectangle(), ref bitmap);

        return bitmap;
    }
    private void ClipRegionFromImage(Bitmap image, Rectangle region, Bitmap destination)
    {
        GraphicsUtils.CopyRegionIntoImage(image, region.ToSystemDrawingRectangle(), ref destination);
        
    }


    private Bitmap CaptureRegionOnScreen(Rectangle region)
    {
        //var b = GameService.Graphics.SpriteScreen.LocalBounds;

        Bitmap bitmap = new(region.Width, region.Height);
        try
        {
            using var g = Graphics.FromImage(bitmap);

            double factor = GameService.Graphics.UIScaleMultiplier;

            g.CopyFromScreen(
                new((int)(region.X * factor), (int)(region.Y * factor)), 
                DrawPoint.Empty, 
                new((int)(region.Width * factor), (int)(region.Height * factor))
            );

        }
        catch
        {

        }

        return bitmap;
    }
    private void CaptureRegionOnScreen(Rectangle region, Bitmap dest)
    {
        //var b = GameService.Graphics.SpriteScreen.LocalBounds;

        try
        {
            using var g = Graphics.FromImage(dest);

            double factor = GameService.Graphics.UIScaleMultiplier;

            g.CopyFromScreen(
                new((int)(region.X * factor), (int)(region.Y * factor)),
                DrawPoint.Empty,
                new((int)(region.Width * factor), (int)(region.Height * factor))
            );

        }
        catch
        {

        }
    }

    //kenedia's capture
    /* private (Bitmap lastImage, Bitmap newImage) CaptureRegion((Bitmap lastImage, Bitmap newImage) images, FramedMaskedRegion region, ScreenRegionType t)
    {
        images.lastImage?.Dispose();
        images.lastImage = images.newImage;

        var b = GameService.Graphics.SpriteScreen.LocalBounds;
        Point maskSize = t switch
        {
            ScreenRegionType.TopLeft => new(100, 25),
            ScreenRegionType.TopRight => new(100, 25),
            ScreenRegionType.BottomLeft => new(100, 25),
            ScreenRegionType.BottomRight => new(100, 25),
            ScreenRegionType.Center => new(100, 100),
            ScreenRegionType.LoadingSpinner => new(100, 100),
            _ => Point.Zero
        };

        Point maskPos = t switch
        {
            ScreenRegionType.TopLeft => new(0, 0),
            ScreenRegionType.TopRight => new(b.Width - maskSize.X, 0),
            ScreenRegionType.BottomLeft => new(0, b.Height - maskSize.Y),
            ScreenRegionType.BottomRight => new(b.Width - maskSize.X, b.Height - maskSize.Y),
            ScreenRegionType.Center => new(b.Center.X - (maskSize.X / 2), b.Center.Y - (maskSize.Y / 2)),
            ScreenRegionType.LoadingSpinner => new(b.Width - maskSize.X, b.Height - maskSize.Y - 50),
            _ => Point.Zero
        };

        region.BorderColor = Color.Transparent;
        region.Location = maskPos;
        region.Size = maskSize;

        try
        {
            RECT wndBounds = ClientWindowService.WindowBounds;
            bool windowed = GameService.GameIntegration.GfxSettings.ScreenMode == Blish_HUD.GameIntegration.GfxSettings.ScreenModeSetting.Windowed;
            RectangleDimensions offset = windowed ? SharedSettings.WindowOffset : new(0);

            Bitmap bitmap = new(maskSize.X, maskSize.Y);
            using var g = Graphics.FromImage(bitmap);
            using MemoryStream s = new();

            double factor = GameService.Graphics.UIScaleMultiplier;
            g.CopyFromScreen(new(wndBounds.Left + offset.Left + (int)(maskPos.X * factor), wndBounds.Top + offset.Top + (int)(maskPos.Y * factor)), DrawPoint.Empty, new((int)(maskSize.X * factor), (int)(maskSize.Y * factor)));

            images.newImage = bitmap;
        }
        catch
        {

        }

        return images;
    }*/

   /* static double CompareImagesMSE((Bitmap lastImage, Bitmap newImage) images)
    {
        var image1 = images.lastImage;
        var image2 = images.newImage;

        if (image1 is null || image2 is null || image1?.Size != image2?.Size)
        {
            return 0;
        }

        double sumSquaredDiff = 0;
        int pixelCount = image1.Width * image1.Height;

        for (int y = 0; y < image1.Height; y++)
        {
            for (int x = 0; x < image1.Width; x++)
            {
                System.Drawing.Color color1 = image1.GetPixel(x, y);
                System.Drawing.Color color2 = image2.GetPixel(x, y);

                int diffR = color1.R - color2.R;
                int diffG = color1.G - color2.G;
                int diffB = color1.B - color2.B;

                sumSquaredDiff += (diffR * diffR) + (diffG * diffG) + (diffB * diffB);
            }
        }

        double mse = sumSquaredDiff / pixelCount;
        double similarity = 1.0 - mse / (255.0 * 255.0 * 3.0); // Normalize to [0, 1]

        return similarity;
    }
*/

}