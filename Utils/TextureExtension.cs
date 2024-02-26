using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System.Drawing;
using System.IO;
using Color = Microsoft.Xna.Framework.Color;

namespace BoonsUp.Utils;

public static class Texture2DExtension
{
    public static Texture2D CreateTexture2D(this MemoryStream s)
    {
        Texture2D texture;

        using (Blish_HUD.Graphics.GraphicsDeviceContext device = Blish_HUD.GameService.Graphics.LendGraphicsDeviceContext())
        {
            texture = Texture2D.FromStream(device.GraphicsDevice, s);
        }

        return texture;
    }

    public static Texture2D CreateTexture2D(this Bitmap bitmap)
    {
        Texture2D texture;
        MemoryStream memStream = new();
        bitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);

        using (Blish_HUD.Graphics.GraphicsDeviceContext device = Blish_HUD.GameService.Graphics.LendGraphicsDeviceContext())
        {
            texture = Texture2D.FromStream(device.GraphicsDevice, memStream);
        }

        return texture;
    }

    public static Texture2D ToGrayScaledPalettable(this Texture2D original)
    {
        if (original == null || original.IsDisposed) return null;

        // make an empty bitmap the same size as original
        var colors = new Color[original.Width * original.Height];
        original.GetData(colors);
        var destColors = new Color[original.Width * original.Height];
        Texture2D newTexture;

        using (Blish_HUD.Graphics.GraphicsDeviceContext device = Blish_HUD.GameService.Graphics.LendGraphicsDeviceContext())
        {
            newTexture = new Texture2D(device.GraphicsDevice, original.Width, original.Height);
        }

        for (int i = 0; i < original.Width; i++)
        {
            for (int j = 0; j < original.Height; j++)
            {
                // get the pixel from the original image
                int index = i + (j * original.Width);
                Color originalColor = colors[index];

                // create the grayscale version of the pixel
                float maxval = .3f + .59f + .11f + .79f;
                float grayScale = (originalColor.R / 255f * .3f) + (originalColor.G / 255f * .59f) + (originalColor.B / 255f * .11f) + (originalColor.A / 255f * .79f);
                grayScale /= maxval;

                destColors[index] = new Color(grayScale, grayScale, grayScale, originalColor.A);
            }
        }

        newTexture.SetData(destColors);
        return newTexture;
    }
    public enum BitMapColorChannel
    {
        Red,
        Green,
        Blue,
        Alpha
    }
    private static byte[,] pixelMap(Bitmap m_bitmap, BitMapColorChannel channel_index)
    {
        int size = m_bitmap.Width * m_bitmap.Height;
        int picture_width = m_bitmap.Width;
        int picture_height = m_bitmap.Height;
        byte[,] pixels_map = new byte[picture_width, picture_height];

        int dimmest = 255;int brightest = 0;int sum = 0;
        for (int i = 0; i < picture_height; i++)
        {
            for (int j = 0; j < picture_width; j++)
            {
                System.Drawing.Color color = m_bitmap.GetPixel(j, i);
                byte color_intensity = 0;
                switch (channel_index)
                {
                    case BitMapColorChannel.Red:
                        color_intensity = color.R;
                        break;
                    case BitMapColorChannel.Green:
                        color_intensity = color.G;
                        break;
                    case BitMapColorChannel.Blue:
                        color_intensity = color.B;
                        break;
                    case BitMapColorChannel.Alpha:
                        color_intensity = color.A;
                        break;
                }
                sum += color_intensity;
                pixels_map[j, i] = color_intensity;
                if(color_intensity < dimmest) dimmest= color_intensity;
                if(color_intensity > brightest) brightest= color_intensity;
            }
        }
        byte average = (byte) (sum / (picture_height * picture_width));
        byte median = (byte)(255 - (brightest - dimmest));
        for (int i = 0; i < picture_height; i++)
        {
            for (int j = 0; j < picture_width; j++)
            {
                pixels_map[j,i] = pixels_map[j,i] > average ? (byte)255 : (byte)0;
            }
        }

        return pixels_map;
    }
    public static Texture2D ToBlueChannel(this Bitmap bitmap)
    {
        
        Texture2D texture;
        MemoryStream s = new();
        byte[,] blue = pixelMap(bitmap, BitMapColorChannel.Blue);
        for(var j=0; j < bitmap.Height; j++)
        {
            for(var i =0; i < bitmap.Width; i++)
            {
                /* var pixel = bitmap.GetPixel(i, j);
                 var clamp = pixel.B > 80 ? 255 : 0;
                 var newColor = System.Drawing.Color.FromArgb(clamp, clamp, clamp);
                 bitmap.SetPixel(i,j,newColor);*/
                bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(blue[i, j], blue[i, j], blue[i, j]));
            }
        }
        bitmap.Save(s, System.Drawing.Imaging.ImageFormat.Bmp);

        using (Blish_HUD.Graphics.GraphicsDeviceContext device = Blish_HUD.GameService.Graphics.LendGraphicsDeviceContext())
        {
            texture = Texture2D.FromStream(device.GraphicsDevice, s);
        }

        return texture;
    }

    public static Texture2D ToBlueChannel(this Texture2D original)
    {
        if (original == null || original.IsDisposed) return null;

        // make an empty bitmap the same size as original
        var colors = new Color[original.Width * original.Height];
        original.GetData(colors);
        var destColors = new Color[original.Width * original.Height];
        Texture2D newTexture;

        using (Blish_HUD.Graphics.GraphicsDeviceContext device = Blish_HUD.GameService.Graphics.LendGraphicsDeviceContext())
        {
            newTexture = new Texture2D(device.GraphicsDevice, original.Width, original.Height);
        }

        for (int i = 0; i < original.Width; i++)
        {
            for (int j = 0; j < original.Height; j++)
            {
                // get the pixel from the original image
                int index = i + (j * original.Width);
                Color originalColor = colors[index];


                destColors[index] = new Color(originalColor.B, originalColor.B, originalColor.B, 1.0f); ;
            }
        }

        newTexture.SetData(destColors);
        return newTexture;
    }


    public static Texture2D SetFromBitmap(this Texture2D texture, Bitmap bitmap)
    {
        byte[] bitmapColors = new byte[bitmap.Width * bitmap.Height * 4];
        var index = 0;
        for(var j = 0; j < bitmap.Height; j++)
        {
            for (var i = 0; i < bitmap.Width; i++)
            {
                var pixel = bitmap.GetPixel(i, j);
                bitmapColors[index++] = (byte)pixel.R;
                bitmapColors[index++] = (byte)pixel.G;
                bitmapColors[index++] = (byte)pixel.B;
                bitmapColors[index++] = (byte)pixel.A;
            }
        }

        texture.SetData(bitmapColors);
        return texture;
    }
}
