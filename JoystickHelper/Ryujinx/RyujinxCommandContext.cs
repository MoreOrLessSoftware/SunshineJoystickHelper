using System.Text.Json;

namespace JoystickHelper.Ryujinx;

internal class RyujinxCommandContext
{
    public required RyujinxSettingsSection Settings { get; internal set; }
    public required string ConfigPath { get; internal set; }
    public required string LastSettingsPath { get; internal set; }

    public RyujinxLastSettings GetLastSettings()
    {
        if (String.IsNullOrEmpty(LastSettingsPath) || !File.Exists(LastSettingsPath))
        {
            throw new Exception("LastSettingsPath is not a valid file path");
        }

        var lastSettingsJson = File.ReadAllText(LastSettingsPath);
        return JsonSerializer.Deserialize<RyujinxLastSettings>(lastSettingsJson) ?? new RyujinxLastSettings();
    }
}