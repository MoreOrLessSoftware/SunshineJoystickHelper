﻿
namespace JoystickHelper.Ryujinx;

internal class RyujinxLastSettings
{
    public RyujinxLastSettings(List<SwappedJoystickResult>? swappedJoysticks = null)
    {
        SwappedJoysticks = swappedJoysticks ?? new List<SwappedJoystickResult>();
    }
    public List<SwappedJoystickResult> SwappedJoysticks { get; } = new List<SwappedJoystickResult>();

    internal IList<JoystickInfo> GetJoystickInfos()
    {
        return SwappedJoysticks.Select(j => new JoystickInfo { SdlJoystickGuid = j.OldInputId }).ToList();
    }
}
