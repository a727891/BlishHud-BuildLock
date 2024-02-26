using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using BoonsUp.Services;
using BoonsUp.Utils;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.WIC;
using System.Collections.Generic;
using System.IO;

namespace BoonsUp.Settings;

public class ModuleMainSettingsView : View
{
    BoonTextureCaptureService textures = new BoonTextureCaptureService();
    Container container = new Panel();

    List<Image> images = new List<Image>();
    public ModuleMainSettingsView()
    {
        textures.TexturesUpdated += Textures_TexturesUpdated;
    }

    private void Textures_TexturesUpdated(object sender, bool e)
    {
        images.ForEach(image => { image?.Dispose(); });
        (var source, var boons) = textures.GetTextures();
        var x = 0;
        var y = 0;
        var z = 0;
        foreach (var s in source)
        {
            if (z >= 12 && z < 25)
            {
                {
                    using MemoryStream strm = new();
                    s.Save(strm, System.Drawing.Imaging.ImageFormat.Bmp);
                    var i = new Image()
                    {
                        Parent = container,
                        Location = new Microsoft.Xna.Framework.Point(x, 50),
                        Size = new Microsoft.Xna.Framework.Point(s.Width, s.Height),
                        Texture = strm.CreateTexture2D()
                    };
                    images.Add(i);
                    x += s.Width + 5;
                    if (s.Height > y) y = s.Height;
                }
            }
            z++;
        }
        y += 10;x = 0;z = 0;
        foreach (var boon in boons)
        {
            if (z >= 12 && z < 25)
            {
                
                        using MemoryStream strm = new();
                boon.Save(strm, System.Drawing.Imaging.ImageFormat.Bmp);
                var i = new Image()
                {
                    Parent = container,
                    Location = new Microsoft.Xna.Framework.Point(x, y),
                    Size = new Microsoft.Xna.Framework.Point(boon.Width, boon.Height),
                    Texture = strm.CreateTexture2D()
                };
                images.Add(i);
                x += boon.Width + 5;
            }
            z++;
            

        }
    }

    protected override void Build(Container buildPanel)
    {
        container = buildPanel;
               
        var btn = new StandardButton()
        {
            Parent = buildPanel,
            Location = new Microsoft.Xna.Framework.Point(0, 0),
            Text = "capture"
        };
        btn.Click += (s, e) =>
        {
            textures.TestCapture();
        };



    }
    public void Dispose()
    {
        textures.TexturesUpdated -= Textures_TexturesUpdated;
        textures.Dispose();
    }
}
