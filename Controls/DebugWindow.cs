using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings.UI.Views;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BoonsUp;
using BoonsUp.Views;

namespace BoonsUp.Controls;

public class DebugWindow : TabbedWindow2
{
    private static Texture2D? Background => Service.Textures?.WindowBackground;


    private CustomDebugView childView = new CustomDebugView();

    //Where on the background texture should the panel render
    private static Rectangle SettingPanelRegion => new()
    {
        Location = new Point(38, 25),
        //Location = new Point(-7, +25),
        //Size = new Point(Background!.Width, Background!.Height),
        Size = new Point(1100, 705),
    };

    private static Rectangle SettingPanelContentRegion => new()
    {
        Location = SettingPanelRegion.Location + new Point(52, 0),
        Size = SettingPanelRegion.Size - SettingPanelRegion.Location,
    };
    //private static Point SettingPanelWindowSize => new(800, 600);

    public DebugWindow() : base(Background, SettingPanelRegion, SettingPanelContentRegion /*, SettingPanelWindowSize*/)
    {
        Id = $"{nameof(Module)}_a6b38a83-4163-4d97-b894-282406b29a48";
        Emblem = Service.Textures?.Cog;
        Parent = GameService.Graphics.SpriteScreen;
        Title = "Debug Window";
        //Subtitle = Strings.SettingsPanel_Subtitle;
        SavesPosition = true;
        //_backgroundColor = new Color(10, 10, 10);

        //Service.Settings.SettingsPanelKeyBind.Value.Activated += (_, _) => ToggleWindow();

        BuildTabs();

#if DEBUG
        Task.Delay(500).ContinueWith(_ => Show());
#endif
    }

    public new void Update(GameTime gametime)
    {
        base.Update(gametime);
        childView.Update(gametime, Visible);
    }

    private void BuildTabs()
    {
        Tabs.Add(
            new Tab(
                Service.Textures?.Cog,
                () => childView,
                "Debug"
            ));
    }
}
