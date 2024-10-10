namespace JoystickHelper;

public class JoystickInfo
{
    public Guid SdlJoystickGuid { get; set; }

    public override string ToString()
    {
        return SdlJoystickGuid.ToString();
    }
}