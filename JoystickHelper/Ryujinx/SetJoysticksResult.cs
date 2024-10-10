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
        public int InputIndex { get; internal set; }
        public Guid OldInputId { get; set; }
    }
}