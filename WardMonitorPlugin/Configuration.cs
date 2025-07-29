using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace WardMonitorPlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public string ApiUrl { get; set; } = "http://localhost:8000/ward-update";
    public bool EnableDataCollection { get; set; } = true;

    [NonSerialized]
    private IDalamudPluginInterface? pluginInterface;

    public void Initialize(IDalamudPluginInterface pInterface)
    {
        pluginInterface = pInterface;
    }

    public void Save()
    {
        pluginInterface?.SavePluginConfig(this);
    }
}
