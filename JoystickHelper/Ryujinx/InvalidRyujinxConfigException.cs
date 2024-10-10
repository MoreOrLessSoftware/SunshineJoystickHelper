namespace JoystickHelper.Ryujinx;

internal class InvalidRyujinxConfigException : Exception
{
    public InvalidRyujinxConfigException(string message, Exception ex) : base(message, ex)
    {
    }
}