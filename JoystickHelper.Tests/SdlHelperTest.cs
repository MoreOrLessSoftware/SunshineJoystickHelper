using System;
using Xunit;
using Xunit.Abstractions;

namespace JoystickHelper.Tests;

public class SdlHelperTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SdlHelperTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestIfDetectsControllers()
    {
        var controllerGuids = SdlHelper.GetConnectedJoysticks();
        
        _testOutputHelper.WriteLine(String.Join(", ", controllerGuids));
        
        Assert.NotEmpty(controllerGuids);
    }
}