using System.Text.Json;
using SerialCOM.Models;

namespace SerialCOM.Services;

/// <summary>
/// Loads/saves <see cref="AppSettings"/> as JSON under
/// %AppData%/SerialCOM/settings.json. Never throws to callers —
/// a corrupt file just yields defaults. Case-insensitive so
/// hand-edited or older PascalCase files still load.
/// </summary>
public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    public string SettingsPath { get; }

    public SettingsService(string? settingsPath = null)
    {
        SettingsPath = settingsPath
            ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SerialCOM", "settings.json");
    }

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
                return new AppSettings();
            string json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
        }
        catch (Exception)
        {
            // Corrupt JSON, locked file, no permission — start fresh.
            return new AppSettings();
        }
    }

    public bool Save(AppSettings settings)
    {
        try
        {
            string? dir = Path.GetDirectoryName(SettingsPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            string json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(SettingsPath, json);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
