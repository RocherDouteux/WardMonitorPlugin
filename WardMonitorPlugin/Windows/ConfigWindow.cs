using ImGuiNET;
using System.Numerics;

namespace WardMonitorPlugin.Windows;

public class ConfigWindow(Plugin plugin)
{
    private bool visible;

    public bool Visible
    {
        get => visible;
        set => visible = value;
    }

    public void Draw()
    {
        if (!Visible)
            return;

        ImGui.SetNextWindowSize(new Vector2(400, 150), ImGuiCond.FirstUseEver);
        if (!ImGui.Begin("Ward Monitor Configuration", ref visible, ImGuiWindowFlags.NoCollapse))
        {
            ImGui.End();
            return;
        }

        var config = plugin.Configuration;

        ImGui.Text("Configure the API endpoint and data collection:");

        var apiUrl = config.ApiUrl;
        if (ImGui.InputText("API URL", ref apiUrl, 512))
        {
            config.ApiUrl = apiUrl;
        }

        var enableDataCollection = config.EnableDataCollection;
        if (ImGui.Checkbox("Enable Data Collection", ref enableDataCollection))
        {
            config.EnableDataCollection = enableDataCollection;
        }

        if (ImGui.Button("Save"))
        {
            config.Save();
        }

        ImGui.End();
    }
}
