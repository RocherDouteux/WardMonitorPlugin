using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WardMonitorPlugin.Models;
using WardMonitorPlugin.Structures;

namespace WardMonitorPlugin;

public class WardScanner : IDisposable
{
    private static readonly Dictionary<uint, string> TerritoryToRegion = new()
    {
        { 339, "Mist" },
        { 340, "The Lavender Beds" },
        { 341, "The Goblet" },
        { 641, "Shirogane" },
        { 979, "Empyreum" }
    };

    private unsafe delegate void HandleHousingWardInfoDelegate(void* agentBase, IntPtr dataPtr);

    [Signature("40 55 53 41 54 41 55 41 57 48 8D AC 24 ?? ?? ?? ?? B8", DetourName = nameof(OnHousingWardInfo))]
    private Hook<HandleHousingWardInfoDelegate>? housingWardInfoHook;

    private readonly Plugin plugin;

    public WardScanner(Plugin plugin)
    {
        this.plugin = plugin;

        try
        {
            Plugin.InteropProvider.InitializeFromAttributes(this);

            if (housingWardInfoHook != null)
            {
                housingWardInfoHook.Enable();
                Plugin.Log.Information("Hook signature resolved successfully.");
            }
            else
            {
                Plugin.Log.Error("Failed to resolve housing ward info hook signature.");
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.Error($"Failed to initialize HousingWardInfo hook: {ex}");
        }
    }

    private unsafe void OnHousingWardInfo(void* agentBase, IntPtr dataPtr)
    {
        housingWardInfoHook!.Original(agentBase, dataPtr);

        if (!plugin.Configuration.EnableDataCollection)
            return;

        Task.Run(() => HandleWardUpdateAsync(dataPtr));
    }

    private Task HandleWardUpdateAsync(IntPtr dataPtr)
    {
        try
        {
            var wardInfo = HousingWardInfo.Read(dataPtr);
            var territoryId = (uint)wardInfo.LandIdent.TerritoryTypeId;
            var wardNumber = (ushort)wardInfo.LandIdent.WardNumber;

            var regionName = TerritoryToRegion.TryGetValue(territoryId, out var name)
                                 ? name
                                 : $"Unknown";

            var plots = new List<Plot>();
            
            var tenantType = wardInfo.TenantType.ToString();


            for (var i = 0; i < 60; i++)
            {
                var entry = wardInfo.HouseInfoEntries[i];

                plots.Add(new Plot
                {
                    PlotNumber = i + 1,
                    Price = (int)entry.HousePrice,
                    Size = PlotSizeHelper.GetPlotSize(regionName, i + 1),
                    Available = (entry.InfoFlags & HousingFlags.PlotOwned) == 0,
                    TenantType = tenantType
                });
            }

            var region = new Region
            {
                RegionName = regionName,
                Wards =
                [
                    new Ward
                    {
                        WardId = wardNumber + 1,
                        Plots = plots
                    }
                ]
            };

            return SendToBackend(region);
        }
        catch (Exception ex)
        {
            Plugin.Log.Error($"Error in HandleWardUpdateAsync: {ex}");
            return Task.CompletedTask;
        }
    }
    
    private async Task SendToBackend(Region region)
    {
        try
        {
            using var http = new HttpClient();
            var json = JsonSerializer.Serialize(region);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await http.PostAsync(plugin.Configuration.ApiUrl, content);
            var result = await response.Content.ReadAsStringAsync();

            Plugin.Log.Information($"Backend responded: {response.StatusCode} - {result}");
        }
        catch (Exception ex)
        {
            Plugin.Log.Error($"Failed to send region data: {ex}");
        }
    }

    public void Dispose()
    {
        housingWardInfoHook?.Dispose();
        Plugin.Log.Information("WardScanner disposed.");
    }
}
