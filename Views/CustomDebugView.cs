using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings;
using Blish_HUD.Settings.UI.Views;
using BoonsUp.Controls;
using BoonsUp.Services;
using Microsoft.Xna.Framework;
using Container = Blish_HUD.Controls.Container;
using BoonsUp.Utils;

namespace BoonsUp.Views;

public class CustomDebugView : View
{
    private BoonMask? _mask=null;
    private Label cnt = new Label();
    private ImageCapture? _imageCapture;

    private bool texturesShouldRefresh = false;

    BoonTextureCaptureService textures = new BoonTextureCaptureService();
    List<Image> images = new List<Image>();

    public CustomDebugView() : base()
    {
    }

    protected override void Build(Container buildPanel)
    {
        base.Build(buildPanel);
        cnt = new Label()
        {
            Parent = buildPanel,
            Location = new Point(0, 50),
            Text = "0",
            AutoSizeWidth = true,
        };

        var maskButton = new StandardButton()
        {
            Parent = buildPanel,
            Location = new Point(0, 0),
            Text = "Show BoonMask",

        };
        maskButton.Click += (s, e) =>
        {
            if (_mask != null)
            {
                _mask?.Dispose();
                _mask = null;
                maskButton.Text = "Show BoonMask";
            }
            else
            {
                maskButton.Text = "Hide BoonMask";
                _mask = new BoonMask()
                {
                    Parent = GameService.Graphics.SpriteScreen
                };
            }
        };


        var btn = new StandardButton()
        {
            Parent = buildPanel,
            Location = new Point(170, 0),
            Text = "capture"
        };
        btn.Click += (s, e) =>
        {

            texturesShouldRefresh = !texturesShouldRefresh;
        };

        _imageCapture = new ImageCapture()
        {
            Parent = buildPanel,
            Location = new Point(300, 0)
        };
        var savedImageButton = new StandardButton()
        {
            Parent = buildPanel,
            Location = new Point(400, 0),
            Text = "5s store img"
        };
        savedImageButton.Click += (s, e) =>
        {
            _imageCapture.ToggleCapture();
        };

        textures.TexturesUpdated += (s, e) => UpdateTextures(buildPanel);
            

    }

    private void UpdateTextures(Container buildPanel)
    {
        if (images.Count > 0) return;

        images.ForEach(image => { image?.Texture?.Dispose(); image?.Dispose(); });
        var source = textures.GetBoonTexture2D();
        //(var source, var boons) = textures.GetTextures();
        //var clippedSource = textures.GetClippedImage();
        var x = 0;
        var y = 100;
        var z = 0;

       
       /* var sot = new Image()
        {
            Parent = buildPanel,
            Location = new Point(x, y),
            Size = new Point(clippedSource.Width*2, clippedSource.Height*2),
            Texture = clippedSource.CreateTexture2D()
        };
        images.Add(sot);

        y += (clippedSource.Height*2) + 10;*/

        for (var i = 0; i < source.Count; i++)
        {
            var img = new Image()
            {
                Parent = buildPanel,
                Location = new Point(x, y),
                Size = new Point(source[i].Width * 2, source[i].Height * 2),
                Texture = source[i]
            };
            images.Add(img);

           /* var img2 = new Image()
            {
                Parent = buildPanel,
                Location = new Point(x, y + 10 + img.Height),
                Size = new Point(boons[i].Width * 2, boons[i].Height * 6),
                Texture = boons[i].ToBlueChannel()
            };
            images.Add(img2);*/

            x += (img.Width) + 5;
            if (i % 12 == 11)
            {
                y += (img.Height) /*+ (img2.Height)*/ + 20;
                x = 0;
            }
        }

    }

    private double throttle=0;
    public void Update(GameTime gametime, bool WindowOpen) {

        _imageCapture.DoUpdate(gametime);

        if (WindowOpen)
        {
            if (!texturesShouldRefresh) return;
            _mask?.DoUpdate(gametime);
            throttle += gametime.ElapsedGameTime.TotalMilliseconds;
            if(throttle > 333) { 
                throttle -= 333;
                textures.TestCapture();
            } 
            cnt.Text = gametime.TotalGameTime.ToString();
        }
    }

}