# What is Sunshine Joystick Helper?

The purpose of this tool is to help with automatic configuration of certain apps that 
store specific gamepad IDs in their configuration file.

This is an issue when streaming games using Sunshine that you also play locally on the 
host PC, as the gamepad IDs for locally connected gamepads differ from those created by Sunshine
for the remote gamepads.

This tool updates the configuration files of certain applications to use whatever gamepad IDs are currently
connected according to Windows - whether they are locally connected or connected remotely via Sunshine.

The currently supported applications are: **Ryujinx**.

# How it works

This tool simply gets the gamepad IDs of all currently connected gamepads using the 
Ryujinx SDL2 library, and replaces the gamepad IDs in the specified application configuration 
file with the currently connected ID(s).

It will do so for all currently connected gamepads, so it does support multi-player
inputs when they have been pre-configured in the application.

This tool does NOT re-configure button mappings, etc; it simply replaces the gamepad hardware IDs. 
So, the assumption is that your button mappings for local and remote gamepads are the same.

# Ryujinx Usage

Set the input IDs to the currently connected devices:
```
JoystickHelper.exe ConfigureRyujinx
```

After you're done streaming, revert the input IDs to what was configured prior to last running `ConfigureRyujinx`:
```
JoystickHelper.exe RevertRyujinx
```

### Sample PowerShell Script Usage Example
Here's a sample PowerShell script that will configure the joysticks in Ryujinx at launch, and then
revert them back to their previous IDs when Ryujinx exits:
```
&"C:\JoystickHelper\JoystickHelper.exe" ConfigureRyujinx
if ($LastExitCode -eq 1) {
    Break
}
&"C:\Ryujinx\Ryujinx.exe"
&"C:\JoystickHelper\JoystickHelper.exe" RevertRyujinx
```
> Be careful about when you run this script. If you run it as a Sunshine command, the gamepads may not
yet be "connected" on the host when the stream starts and therefore won't be swapped. I've found it more reliable to use this
script when launching Ryujinx from a launcher/frontend.

### Optional Configuration
If your Ryujinx configuration file (`Config.json`) is not in the default `AppData\Ryujinx` location, you can tell
JoystickHelper where to find it by setting it in the `settings.json` file in the `JoystickHelper.exe` directory, eg.:
```
{
  "RyujinxSettings": {
    "ConfigPath": "C:\\PathToRyujinxData\\Config.json"
  }
}
```
or by passing it on the command line:
```
JoystickHelper.exe ConfigureRyujinx --ConfigPath "C:\\PathToRyujinxData\\Config.json"
```