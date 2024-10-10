using CommandLine;

namespace JoystickHelper.Ryujinx;

[Verb("RevertRyujinx", HelpText = "Reverts the input device IDs in the Ryujix configuration file to their previous values.")]
internal class RevertRyujinxOptions : RyujinxOptionsBase
{
}