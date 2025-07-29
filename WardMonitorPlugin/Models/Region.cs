using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WardMonitorPlugin.Models;

public class Region
{
    [JsonPropertyName("region_name")]
    public string RegionName { get; set; } = string.Empty;

    [JsonPropertyName("wards")]
    public List<Ward> Wards { get; set; } = new();
}
