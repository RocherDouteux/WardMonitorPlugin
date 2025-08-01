using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using WardMonitorPlugin.Models;
using WardMonitorPlugin.Windows;

namespace WardMonitorPlugin;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    
    [PluginService] internal static IGameInteropProvider InteropProvider { get; private set; } = null!;
    
    
    public Configuration Configuration { get; private set; }
    
    public readonly ConfigWindow ConfigWindow;
    
    private readonly WardScanner? wardScanner;

    
    
    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        ConfigWindow = new ConfigWindow(this);

        CommandManager.AddHandler("/wmc", new CommandInfo(ShowConfigurationCommand)
        {
            HelpMessage = "Open configuration window for Ward Monitor Plugin."
        });
        
        CommandManager.AddHandler("/fakesend", new CommandInfo(FakeSendCommand)
        {
            HelpMessage = "Fake send to API Url ---"
        });
        
        PluginInterface.UiBuilder.Draw += DrawConfigUI;
        
        wardScanner = new WardScanner(this);
        Log.Information("[WardMonitorPlugin] Initialized.");
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler("/wmc");
        PluginInterface.UiBuilder.Draw -= DrawConfigUI;
        wardScanner?.Dispose();
    }

    private void ShowConfigurationCommand(string command, string args)
    {
        ConfigWindow.Visible = !ConfigWindow.Visible;
    }
    
    private void FakeSendCommand(string command, string args)
    {
        if (!Configuration.EnableDataCollection || string.IsNullOrWhiteSpace(Configuration.ApiUrl))
        {
            Log.Warning("Data collection is disabled or API URL is not set.");
            return;
        }

        var testRegion = new Region
        {
            RegionName = "Test Region",
            Wards =
            [
                new Ward()
                {
                    WardId = 99,
                    Plots = Enumerable.Range(1, 60).Select(i => new Plot
                    {
                        PlotNumber = i,
                        Size = "L",
                        Price = 1_000_000 + (i * 1000),
                        Available = i % 2 == 0
                    }).ToList()
                }
            ]
        };

        try
        {
            using var http = new HttpClient();
            var json = JsonSerializer.Serialize(testRegion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = http.PostAsync(Configuration.ApiUrl, content);
            var result = response.Result.Content.ReadAsStringAsync();

            Log.Information($"[FakeSend] Backend responded: {response.Result.StatusCode} - {result}");
        }
        catch (Exception ex)
        {
            Log.Error($"[FakeSend] Error: {ex}");
        }
    }

    public void DrawConfigUI()
    {
        ConfigWindow.Draw();
    }
    
}
