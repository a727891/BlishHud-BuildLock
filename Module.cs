using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Modules;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using BoonsUp.Settings;
using BoonsUp.Services;
using Blish_HUD.GameIntegration;
using Gw2Sharp.WebApi.V2.Models;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using BoonsUp.Controls;
using Blish_HUD.Controls;
using System.Drawing;

namespace BoonsUp;


[Export(typeof(Blish_HUD.Modules.Module))]
public class Module : Blish_HUD.Modules.Module
{
    private int _i = 0;
    public static string DIRECTORY_PATH = "BoonsUp"; //Defined folder in manifest.json

    internal static readonly Logger ModuleLogger = Logger.GetLogger<Module>();

    private CornerIcon? _cornerIcon;

    //private BoonMask? _mask;

    [ImportingConstructor]
    public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
    {
        Service.ModuleInstance = this;
        Service.ContentsManager = moduleParameters.ContentsManager;
        Service.Gw2ApiManager = moduleParameters.Gw2ApiManager;
        Service.DirectoriesManager = moduleParameters.DirectoriesManager;
    }

    protected override void DefineSettings(SettingCollection settings) => Service.Settings = new SettingService(settings);

    public override IView GetSettingsView() => new Settings.ModuleMainSettingsView();

    protected override void Initialize()
    {
        this.TEMP_FIX_SetTacOAsActive();
    }

    protected override Task LoadAsync()
    {

        Service.Gw2ApiManager.SubtokenUpdated += Gw2ApiManager_SubtokenUpdated;
        Service.Persistance = Persistance.Load();
        Service.Textures = new TextureService(Service.ContentsManager);
        Service.DebugWindow = new DebugWindow();

        if (Service.Gw2ApiManager.HasPermissions(AccountNameService.NecessaryApiTokenPermissions))
        {
            UpdateAccountName();
        }

        _cornerIcon = new CornerIcon()
        {
            Icon = Service.Textures.Cog,
        };
        _cornerIcon.Click += (s, e) => Service.DebugWindow.ToggleWindow();

        /*_imageCapture = new()
        {
            Parent = GameService.Graphics.SpriteScreen
        };*/
        /*_mask = new BoonMask()
        {
            Parent = GameService.Graphics.SpriteScreen
        };*/

      
        return Task.CompletedTask;

    }

 


    private void TEMP_FIX_SetTacOAsActive()
    {
        // SOTO Fix
        if (Program.OverlayVersion < new SemVer.Version(1, 1, 0))
        {
            try
            {
                var tacoActive = typeof(TacOIntegration).GetProperty(nameof(TacOIntegration.TacOIsRunning)).GetSetMethod(true);
                tacoActive?.Invoke(GameService.GameIntegration.TacO, new object[] { true });
            }
            catch { /* NOOP */ }
        }
    }

    protected override void Unload()
    {
        try
        {
            _cornerIcon?.Dispose();
            Service.Gw2ApiManager.SubtokenUpdated -= Gw2ApiManager_SubtokenUpdated;

            Service.DebugWindow?.Dispose();
            Service.ContentsManager?.Dispose();
            Service.Textures?.Dispose();
            Service.ResetWatcher?.Dispose();

            //_mask?.Dispose();

        }
        catch (Exception e)
        {
            ModuleLogger.Warn("BoonsUp threw exception in Unload, " + e.Message);
        }   
    }

    protected override void Update(GameTime gameTime)
    {
        //_imageCapture?.DoUpdate(gameTime);
        /*_i++;
        if(_i >= 120)
        {
            _i = 0;
            //_mask?.DoUpdate(gameTime);
        }*/
        Service.DebugWindow.Update(gameTime);
    }

    private Microsoft.Xna.Framework.Point GetBoonBottomLeftAnchor()
    {
        var res = GameService.Graphics.SpriteScreen.Size;
        return new Microsoft.Xna.Framework.Point( res.X / 2 + 75, res.Y - 135);
    }

    private void Gw2ApiManager_SubtokenUpdated(object sender, ValueEventArgs<IEnumerable<TokenPermission>> e)
    {
        UpdateAccountName();
    }

    private void UpdateAccountName()
    {
       /* Task.Run(async () =>
        {
            //Service.CurrentAccountName = await AccountNameService.UpdateAccountName();
            Debug.WriteLine("New API token, accountName = " + Service.CurrentAccountName);
        });*/
    }

    private void Capture()
    {
        try
        {
            /*RECT wndBounds = ClientWindowService.WindowBounds;
            bool windowed = GameService.GameIntegration.GfxSettings.ScreenMode == Blish_HUD.GameIntegration.GfxSettings.ScreenModeSetting.Windowed;
            RectangleDimensions offset = windowed ? SharedSettings.WindowOffset : new(0);

            Bitmap bitmap = new(maskSize.X, maskSize.Y);
            using var g = Graphics.FromImage(bitmap);
            using MemoryStream s = new();

            double factor = GameService.Graphics.UIScaleMultiplier;
            g.CopyFromScreen(new(wndBounds.Left + offset.Left + (int)(maskPos.X * factor), wndBounds.Top + offset.Top + (int)(maskPos.Y * factor)), DrawPoint.Empty, new((int)(maskSize.X * factor), (int)(maskSize.Y * factor)));

            images.newImage = bitmap;*/
        }
        catch
        {

        }
    }

}