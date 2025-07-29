using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WardMonitorPlugin.Models;

public class Ward
{
    [JsonPropertyName("ward_id")]
    public int WardId { get; set; }

    [JsonPropertyName("plots")]
    public List<Plot> Plots { get; set; } = new();
}
