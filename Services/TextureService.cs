using System;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace BoonsUp.Services;

public class TextureService : IDisposable
{
    public TextureService(ContentsManager contentsManager)
    {
        WindowBackground = contentsManager.GetTexture(@"background.png");
        Cog = contentsManager.GetTexture(@"cog.png");
        //ShineTexture = contentsManager.GetTexture(@"965696.png");
    }

    public void Dispose()
    {
        WindowBackground.Dispose();
        Cog.Dispose();
        //ShineTexture.Dispose();
    }

    public Texture2D WindowBackground { get; }//925822
    public Texture2D Cog { get; }//925823
    //public Texture2D ShineTexture { get; }//965696


}