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

        Log.Information("Found {count} input configs in Ryujinx config file", inputConfigs.Count);

        for (int i = 0; i < inputConfigs.Count; i++)
        {
            var inputConfig = inputConfigs[i];
            if (joysticks.Count - 1 >= i)
            {
                var oldId = inputConfig["id"]?.ToString();
                var newId = $"{i}-{joysticks[i].SdlJoystickGuid}";

                if (newId != oldId)
                {
                    inputConfig["id"] = newId;

                    if (IsValidRyujinxInputDeviceId(oldId))
                    {
                        result.SwappedJoysticks.Add(new SwappedJoystickResult { InputIndex = i, OldInputId = ConvertRyujinxInputDeviceIdToGuid(oldId) });
                    }

                    Log.Information("Swapped input #{index}: {oldId} >> {newId}", i, oldId, newId);
                }
                else
                {
                    Log.Information("No change needed for input #{index} ({oldId})", i, oldId);
                }
            }
            else
            {
                Log.Information("No connected joystick available to swap for input #{index}", i);
            }
        }

        result.ModifiedConfigJson = JsonConvert.SerializeObject(configData, Formatting.Indented);

        return result;
    }

    public static bool IsValidRyujinxInputDeviceId(string? deviceId)
    {
        return deviceId != null && Regex.IsMatch(deviceId, @"\d-.+");
    }

    public static Guid ConvertRyujinxInputDeviceIdToGuid(string value)
    {
        if (!RyujinxConfigHelper.IsValidRyujinxInputDeviceId(value))
            throw new ArgumentException($"OldRyujinxInputId is not in the expected format: {value}");

        return Guid.Parse(value.Substring(2));
    }
}