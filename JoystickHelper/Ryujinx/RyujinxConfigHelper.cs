using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Text.RegularExpressions;

namespace JoystickHelper.Ryujinx;

public class RyujinxConfigHelper
{
    public static SetJoysticksResult SetJoystickGuids(IList<JoystickInfo> joysticks, string configJson)
    {
        var result = new SetJoysticksResult();

        JObject? configData;
        JArray? inputConfigs;
        try
        {
            configData = JsonConvert.DeserializeObject(configJson) as JObject;
            if (configData == null)
            {
                throw new Exception("Config json was deserialized to null");
            }

            inputConfigs = configData["input_config"] as JArray;
            if (inputConfigs == null)
            {
                throw new Exception("Expected 'input_config' to be an array");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidRyujinxConfigException("Failed to deserialize the Ryujinx config json", ex);
        }

        var inputConfigsToProcess = inputConfigs
            .Where(c => c["backend"]?.ToString() == "GamepadSDL2")
            .OrderBy(c => ParsePlayerIndex(c["player_index"]?.ToString()))
            .ToList();

        if (inputConfigsToProcess.Count == 0)
        {
            Log.Warning("No inputs configured with GamepadSDL2 backend type were found in Ryujinx");
            return result;
        }

        Log.Information("Found {count} SDL2 inputs configured in Ryujinx", inputConfigsToProcess.Count);

        var inputConfigModified = false;

        for (var i = 0; i < inputConfigsToProcess.Count; i++)
        {
            var inputConfig = inputConfigsToProcess[i];
            var playerIndex = inputConfig["player_index"]?.ToString();

            if (i > joysticks.Count - 1)
            {
                Log.Information("No connected joystick available to swap for {playerIndex}", playerIndex);
                continue;
            }

            var oldId = inputConfig["id"]?.ToString();
            var newId = $"{joysticks[i].SdlDeviceIndex}-{joysticks[i].SdlJoystickGuid}";

            if (newId != oldId)
            {
                inputConfig["id"] = newId;
                inputConfigModified = true;

                if (IsValidRyujinxInputDeviceId(oldId))
                {
                    result.SwappedJoysticks.Add(new SwappedJoystickResult { OldRyujinxDeviceId = oldId });
                }

                Log.Information("Swapped input {playerIndex}: {oldId} >> {newId}", playerIndex, oldId, newId);
            }
            else
            {
                Log.Information("No change needed for input {playerIndex} ({oldId})", playerIndex, oldId);
            }
        }

        if (inputConfigModified)
        {
            result.ModifiedConfigJson = JsonConvert.SerializeObject(configData, Formatting.Indented);
        }

        return result;
    }

    private static int ParsePlayerIndex(string? jsonValue)
    {
        if (String.IsNullOrEmpty(jsonValue) || !Regex.IsMatch(jsonValue, @"Player\d"))
        {
            throw new Exception($"PlayerIndex value did not match expected pattern: {jsonValue}");
        }

        return int.Parse(Regex.Match(jsonValue, @"Player(\d)").Groups[1].Value);
    }

    public static bool IsValidRyujinxInputDeviceId(string? deviceId)
    {
        return deviceId != null && Regex.IsMatch(deviceId, @"\d-.+");
    }
}