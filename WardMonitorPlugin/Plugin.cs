using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using WardMonitorPlugin.Windows;

namespace WardMonitorPlugin;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    
    public Configuration Configuration { get; private set; }
    
    public readonly ConfigWindow ConfigWindow;
    
    
    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        ConfigWindow = new ConfigWindow(this);

        CommandManager.AddHandler("/wmc", new CommandInfo(OnCommand)
        {
            HelpMessage = "Open configuration window for Ward Monitor Plugin."
        });
        
        PluginInterface.UiBuilder.Draw += DrawConfigUI;
        
        Log.Information("[WardMonitorPlugin] Initialized.");
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler("/wmc");
        PluginInterface.UiBuilder.Draw -= DrawConfigUI;

    }

    private void OnCommand(string command, string args)
    {
        ConfigWindow.Visible = !ConfigWindow.Visible;
    }

    public void DrawConfigUI()
    {
        ConfigWindow.Draw();
    }
    
}
