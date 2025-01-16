using CustomAvatarLoader.Settings;

namespace CustomAvatarLoader;

using BepInEx.Configuration;
using BepInEx.Logging;
using CustomAvatarLoader.Helpers;
using CustomAvatarLoader.Modules;
using Logging;
using MelonLoader;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UnityEngine;
using Versioning;
using ILogger = Logging.ILogger;

public class Core : MonoBehaviour
{
    protected const string RepositoryName = "YusufOzmen01/desktopmate-custom-avatar-loader";

    protected virtual ILogger Logger { get; private set; }

    protected virtual ISettingsProvider SettingsProvider { get; private set; }

    protected virtual IServiceProvider ServiceProvider { get; private set; }

    protected virtual IEnumerable<IModule> Modules { get; private set; }

    public void InitMelonLoader(MelonLogger.Instance loggerInstance)
    {
        var services = new ServiceCollection();
        ConfigureServices(services, loggerInstance);
        ServiceProvider = services.BuildServiceProvider();
        Init();
    }

    public void InitBepInEx(ManualLogSource loggerInstance, ConfigFile config)
    {
        var services = new ServiceCollection();
        ConfigureServices(services, loggerInstance, config);
        ServiceProvider = services.BuildServiceProvider();
        Init();
    }

    protected virtual void ConfigureServices(IServiceCollection services, MelonLogger.Instance loggerInstance)
    {
        services.AddSingleton(typeof(MelonLogger.Instance), loggerInstance);
        services.AddSingleton(typeof(ISettingsProvider), new MelonLoaderSettings("settings"));
        services.AddScoped(typeof(ILogger), typeof(MelonLoaderLogger));
        services.AddScoped(typeof(IModule), typeof(VrmLoaderModule));
    }

    protected virtual void ConfigureServices(IServiceCollection services, ManualLogSource loggerInstance, ConfigFile config)
    {
        services.AddSingleton(typeof(ManualLogSource), loggerInstance);
        services.AddSingleton(typeof(ISettingsProvider), new BepInExSettings("settings", config));
        services.AddScoped(typeof(ILogger), typeof(BepInExLogger));
        services.AddScoped(typeof(IModule), typeof(VrmLoaderModule));
    }

    private void Init()
    {
        Modules = ServiceProvider.GetServices<IModule>();
        Logger = ServiceProvider.GetService<ILogger>();
        SettingsProvider = ServiceProvider.GetService<ISettingsProvider>();

        var versionChecker = new GitHubVersionChecker(RepositoryName, Logger);
        var updater = new Updater(RepositoryName, Logger);

        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0";

        if (currentVersion == "0")
        {
            Logger.Warn("CurrentVersion is 0, faulty module version?");
        }

        var hasLatestVersion = versionChecker.IsLatestVersionInstalled(currentVersion);

        if (!hasLatestVersion)
        {
            updater.ShowUpdateMessageBox();
        }
        else
        {
            Logger.Info("[VersionCheck] Latest version installed");
        }

        WindowHelper.SetWindowForeground(WindowHelper.GetUnityGameHwnd());

        var playerLogManager = new PlayerLogManager(SettingsProvider, Logger);

        string logPath = Path.Join(Environment.GetEnvironmentVariable("USERPROFILE"), "Appdata", "LocalLow",
            "infiniteloop", "DesktopMate");

        string playerLog = Path.Join(logPath, "Player.log");
        string playerPrevLog = Path.Join(logPath, "Player-prev.log");

        playerLogManager.ClearLog(playerLog);
        playerLogManager.ClearLog(playerPrevLog);
        
        foreach (var module in Modules)
        {
            module.OnInitialize();
        }
    }

    private void Update()
    {
        foreach (var service in Modules)
        {
            service.OnUpdate();
        }
    }
}