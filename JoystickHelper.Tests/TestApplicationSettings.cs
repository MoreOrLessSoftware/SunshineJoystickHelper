using JoystickHelper.Ryujinx;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace JoystickHelper.Tests
{
    public class TestApplicationSettings
    {
        [Fact]
        public void TestApplicationSettingsWork()
        {
            var config = new ConfigurationBuilder().AddJsonFile("./settings.json").Build();
            var ryujinxSettings = config.GetSection("RyujinxSettings").Get<RyujinxSettingsSection>();

            Assert.NotNull(ryujinxSettings);
            Assert.NotNull(ryujinxSettings.ConfigPath);
            Assert.NotEmpty(ryujinxSettings.ConfigPath);
        }
    }
}
