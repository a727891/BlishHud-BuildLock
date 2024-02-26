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
using BoonsUp.Enum;
using System.Linq;

namespace BoonsUp.Services;

public class BoonDetectionService : IDisposable
{
    private BoonTextureCaptureService _boonTextureCaptureService;

    private int BLUE_THRESHOLD = 200;
    public BoonDetectionService(BoonTextureCaptureService _textures)
    {
        _boonTextureCaptureService = _textures;
        _boonTextureCaptureService.TexturesUpdated += _boonTextureCaptureService_TexturesUpdated;
    }

    public void DoUpdate(GameTime gametime)
    {

    }

    private void _boonTextureCaptureService_TexturesUpdated(object sender, bool e)
    {
        (Bitmap[] boons, Bitmap[] trimmed) = _boonTextureCaptureService.GetTextures();
        List<Boons> detectedBoons = DetectBoons(trimmed);
    }


    public void Dispose()
    {
        _boonTextureCaptureService.TexturesUpdated -= _boonTextureCaptureService_TexturesUpdated;
    }

    private List<Boons> DetectBoons(Bitmap[] trimmed)
    {
        List<Boons> result = Enumerable.Repeat(Boons.None, trimmed.Length).ToList();

        for(int i=0; i < trimmed.Length; i++)
        {
            result[i] = BlueChannelStuff(trimmed[i]);
        }
        return result;
    }
 
    private Boons BlueChannelStuff(Bitmap texture)
    {
        bool edgeFound = false;
        List<int> edgeRuns = new List<int>();
        bool OnDark = true;
        int currentRun = 0;
        for(int i = 0; i <= texture.Width; i++)
        {
            var blue = texture.GetPixel(i, 0).B;

            if(!edgeFound && blue < BLUE_THRESHOLD)
            {
                edgeFound = true;
                currentRun = 1;
            }
            else if (edgeFound)
            {
                if(OnDark ? blue < BLUE_THRESHOLD : blue > BLUE_THRESHOLD)
                {
                    currentRun++;
                }
                else
                {
                    edgeRuns.Add(currentRun);
                    currentRun = 0;
                    OnDark = !OnDark;
                }
            }
        }


        return Boons.None;

    }

    

}