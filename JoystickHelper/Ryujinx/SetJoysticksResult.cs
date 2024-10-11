using System.Text.RegularExpressions;

namespace JoystickHelper.Ryujinx
{
    public class SetJoysticksResult
    {
        public string? ModifiedConfigJson { get; set; }
        public List<SwappedJoystickResult> SwappedJoysticks { get; } = new List<SwappedJoystickResult>();
    }

    public class SwappedJoystickResult
    {
        public string OldRyujinxDeviceId { get; set; }

        public JoystickInfo ToJoystickInfo()
        {
            var match = Regex.Match(OldRyujinxDeviceId, @"(?<index>\d)-(?<guid>.+)");
            if (!match.Success)
            {
                return null;
            }

            return new JoystickInfo
            {
                SdlJoystickGuid = Guid.Parse(match.Groups["guid"].Value),
                SdlDeviceIndex = int.Parse(match.Groups["index"].Value)
            };
        }
    }
}