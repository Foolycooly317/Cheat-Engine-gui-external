using System.Text.Json.Serialization;

namespace CheatEngineExternalGui;

public sealed class ToggleDefinition
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Address { get; init; } = "0x0";

    [JsonPropertyName("on")]
    public string OnBytes { get; init; } = string.Empty;

    [JsonPropertyName("off")]
    public string OffBytes { get; init; } = string.Empty;
}
