using CommandLine;

namespace JoystickHelper.Ryujinx
{
    [Verb("RevertRyujinx", false, null, HelpText = "Reverts the input device IDs in the Ryujix configuration file to their previous values.")]
    internal class RyujinxOptionsBase
    {
        [Option("ConfigPath", HelpText = "Specifies the path the Ryujix config file. Required, unless the value is configured in the JoystickHelper settings file.")]
        public string? RyujinxConfigPath { get; set; }
    }
}