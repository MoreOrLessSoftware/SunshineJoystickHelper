using CommandLine;
using JoystickHelper.Ryujinx;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;
using System.Text.Json;

namespace JoystickHelper;

public class Program
{
    private static IConfigurationRoot _config;

    static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Debug()
             .WriteTo.Console()
             .WriteTo.File(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "JoystickHelper.log.txt"))
             .CreateLogger();

        _config = new ConfigurationBuilder().AddJsonFile("./settings.json").Build();

        var result = Parser.Default.ParseArguments<ConfigureRyujinxOptions, RevertRyujinxOptions>(args)
            .MapResult(
                (ConfigureRyujinxOptions opts) => RunConfigureRyujinx(opts),
                (RevertRyujinxOptions opts) => RunRevertRyujinx(opts),
                _ => 1);

        return result;
    }

    private static int RunConfigureRyujinx(ConfigureRyujinxOptions opts)
    {
        Log.Information("Starting ConfigureRyujinx");

        try
        {
            var context = InitRyujinxContext(opts);
            var configJson = File.ReadAllText(context.ConfigPath);
            var joysticks = SdlHelper.GetConnectedJoysticks();
            var setResult = RyujinxConfigHelper.SetJoystickGuids(joysticks, configJson);
            if (!String.IsNullOrEmpty(setResult.ModifiedConfigJson))
            {
                File.WriteAllText(context.ConfigPath, setResult.ModifiedConfigJson);
                File.WriteAllText(context.LastSettingsPath, JsonSerializer.Serialize(new RyujinxLastSettings(setResult.SwappedJoysticks)));

                Log.Information("Ryujinx config file updated");
            }
            return 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to update Ryujinx config");
            return 1;
        }
        finally
        {
            Log.Information("Finished ConfigureRyujinx");
        }
    }

    private static int RunRevertRyujinx(RevertRyujinxOptions opts)
    {
        Log.Information("Starting RevertRyujinx");

        try
        {
            var context = InitRyujinxContext(opts);
            var configJson = File.ReadAllText(context.ConfigPath);
            var joysticks = context.GetLastSettings().GetJoystickInfos();
            var setResult = RyujinxConfigHelper.SetJoystickGuids(joysticks, configJson);
            if (!String.IsNullOrEmpty(setResult.ModifiedConfigJson))
            {
                File.WriteAllText(context.ConfigPath, setResult.ModifiedConfigJson);

                Log.Information("Ryujinx config file updated");
            }
            return 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to revert Ryujinx config");
            return 1;
        }
        finally
        {
            Log.Information("Finished RevertRyujinx");
        }
    }

    private static RyujinxCommandContext InitRyujinxContext(RyujinxOptionsBase opts)
    {
        var settings = _config.GetSection("RyujinxSettings").Get<RyujinxSettingsSection>();

        var configPath = opts.RyujinxConfigPath ?? settings?.ConfigPath;

        // If not path explicitly specified, use the default Ryujinx location in AppData
        if (String.IsNullOrEmpty(configPath))
        {
            configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ryujinx\\Config.json");
        }

        if (String.IsNullOrEmpty(configPath))
        {
            throw new Exception("No configuration path for Ryujinx was provided. It should be passed as an input argument, or configured in the settings file.");
        }

        if (!File.Exists(configPath))
        {
            throw new Exception("No configuration file exists at the path: " + configPath);
        }

        var lastSettingsPath = Path.Combine(Path.GetDirectoryName(configPath), "JoystickHelper.lastsettings.json");

        return new RyujinxCommandContext
        {
            Settings = settings,
            ConfigPath = configPath,
            LastSettingsPath = lastSettingsPath
        };
    }
}
