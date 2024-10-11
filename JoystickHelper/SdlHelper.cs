using SDL2;
using Serilog;

namespace JoystickHelper;

public static partial class SdlHelper
{
    public static IList<JoystickInfo> GetConnectedJoysticks(int maxJoysticks = 4)
    {
        // Initialize SDL
        if (SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK) < 0)
        {
            throw new Exception(String.Format("SDL could not initialize! SDL_Error: {0}", SDL.SDL_GetError()));
        }

        // Get number of joysticks
        var numJoysticks = SDL.SDL_NumJoysticks();
        
        var joysticks = new List<JoystickInfo>();

        // Loop and get guids for all joysticks
        for (int i = 0; i < Math.Min(numJoysticks, maxJoysticks); i++)
        {
            var joystick = SDL.SDL_JoystickOpen(i);
            if (joystick == IntPtr.Zero)
            {
                throw new Exception(String.Format("Unable to open joystick {0}. SDL_Error: {1}", i, SDL.SDL_GetError()));
            }
            else
            {
                joysticks.Add(new JoystickInfo { SdlJoystickGuid = SDL.SDL_JoystickGetGUID(joystick), SdlDeviceIndex = i });
            }   
        }
        
        // Quit SDL
        SDL.SDL_Quit();

        if (joysticks.Count > 0)
        {
            Log.Information("SDL retrieved {count} joysticks: {joysticks}", joysticks.Count, joysticks);
        }
        else
        {
            Log.Warning("No joysticks were detected by the SDL library");
        }

        return joysticks;
    }
}