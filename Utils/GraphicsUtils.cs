using System;
using System.Drawing;

namespace BoonsUp.Utils;

public static class GraphicsUtils
{

    public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap)
    {
        CopyRegionIntoImage(srcBitmap, srcRegion, ref destBitmap, new Rectangle(0,0, srcRegion.Width, srcRegion.Height));
    }
    public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
    {
        using (Graphics grD = Graphics.FromImage(destBitmap))
        {
            grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
        }
    }
}