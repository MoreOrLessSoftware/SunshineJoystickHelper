using CommandLine;

namespace JoystickHelper.Ryujinx;

[Verb("ConfigureRyujinx", HelpText = "Sets the input device IDs in the Ryujix configuration file to the IDs of the currently connected devices.")]
internal class ConfigureRyujinxOptions : RyujinxOptionsBase
{
}