namespace JoystickHelper;

public class JoystickInfo
{
    public Guid SdlJoystickGuid { get; set; }
    public int SdlDeviceIndex { get; internal set; }

    public override string ToString()
    {
        return SdlDeviceIndex + "-" + SdlJoystickGuid.ToString();
    }
}