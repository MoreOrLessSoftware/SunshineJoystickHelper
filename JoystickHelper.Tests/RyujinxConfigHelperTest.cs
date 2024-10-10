using System;
using System.Collections.Generic;
using System.IO;
using JoystickHelper.Ryujinx;
using Xunit;

namespace JoystickHelper.Tests;

public class RyujinxConfigHelperTest
{

    [Fact]
    public void TestRyujinxConfig()
    {
        var configJson = File.ReadAllText("./RyujinxConfigSample.json");
        var joysticks = new List<JoystickInfo> { new JoystickInfo { SdlJoystickGuid = Guid.NewGuid() } };
        var result = RyujinxConfigHelper.SetJoystickGuids(joysticks, configJson);

        Assert.Contains(joysticks[0].SdlJoystickGuid.ToString(), result.ModifiedConfigJson);
    }
}