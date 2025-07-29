using System.Text.Json.Serialization;

namespace WardMonitorPlugin.Models;

public class Plot
{
    [JsonPropertyName("plot_number")]
    public int PlotNumber { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }

    [JsonPropertyName("size")]
    public string Size { get; set; } = string.Empty;

    [JsonPropertyName("available")]
    public bool Available { get; set; } = true;
}
