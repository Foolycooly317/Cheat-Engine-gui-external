namespace CheatEngineExternalGui;

public sealed class AppConfig
{
    public string ProcessName { get; init; } = string.Empty;
    public List<ToggleDefinition> Toggles { get; init; } = new();
}
