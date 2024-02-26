using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Blish_HUD;


namespace BoonsUp.Services;

public static class BoonCoordinatesService
{
    private static int _boonRows = 3;
    private static int _boonCols = 12;

    private static int _boonSizeX = 32;
    private static int _boonSizeY = 32;
    
    private static int _boonPadX = 0;
    private static int _boonPadY = 0;

    private static Point _magicOffset = new Point(72, -209); //Position from bottom center of screen to top-left of boons

    public static Point GetBoonSize()
    {
        return new Point(_boonSizeX, _boonSizeY);
    }

    public static int GetTotalBoonCount()
    {
        return _boonRows * _boonCols;
    }

    public static Rectangle OnScreenBoonBounds()
    {
        var spriteScreenSize = GameService.Graphics.SpriteScreen.Size;

        return new Rectangle(
            (spriteScreenSize.X / 2) + _magicOffset.X, 
            spriteScreenSize.Y + _magicOffset.Y,
            (int)((_boonSizeX + _boonPadX) * (_boonCols) / 0.897), 
            (int)((_boonSizeY + _boonPadY) * (_boonRows) / 0.897)
        );
    }

    public static List<Rectangle> GetBoonCoordinates()
    {
        return GetBoonCoordinates(new Point(0, 0));
    }

    public static List<Rectangle> GetBoonCoordinates(Point offset)
    {
        List<Rectangle> locations = new List<Rectangle>();

        int currX = 0; int currY = offset.Y;
        for(int j = 0; j < _boonRows; j++)
        {
            currX = offset.X;
            for(int i = 0; i < _boonCols; i++)
            {
                locations.Add(new Rectangle(currX, currY, _boonSizeX, _boonSizeY));

                currX += _boonSizeX + _boonPadX;
                //currX -= (i % 2 == 0 ? 0 : 1);//Padding Grid offset adjustments
                //currX += (i % 8 != 0 ? 0 : 1);
            }
            currY += _boonSizeY + _boonPadY;
        }

        return locations;
    }

}