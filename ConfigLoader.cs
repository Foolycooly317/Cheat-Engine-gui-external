using System.Text.Json;

namespace CheatEngineExternalGui;

internal static class ConfigLoader
{
    public static AppConfig Load(string configPath)
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Config file not found: {configPath}");
        }

        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (config is null)
        {
            throw new InvalidOperationException("Unable to parse config file.");
        }

        if (string.IsNullOrWhiteSpace(config.ProcessName))
        {
            throw new InvalidOperationException("Config 'processName' is required.");
        }

        return config;
    }
}
