using Microsoft.Xna.Framework;
namespace BoonsUp.Utils;

public static class RectangleExtension
{
    public static System.Drawing.Rectangle ToSystemDrawingRectangle(this Rectangle rect)
    {
        return new(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static System.Drawing.Size ToSystemDrawingSize(this Rectangle rect)
    {
        return new System.Drawing.Size(rect.Width, rect.Height);
    }
   
    
}
