namespace CustomAvatarLoader;

using CustomAvatarLoader.Helpers;
using CustomAvatarLoader.Settings;
using Logging;
using System.Reflection;
using Versioning;
using ILogger = Logging.ILogger;
using CustomAvatarLoader.Modules;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

public static class Core
{
    private const string RepositoryName = "YusufOzmen01/desktopmate-custom-avatar-loader";

    private static ILogger Logger;

    public static ISettingsProvider Settings { get; private set; }
    public static VrmLoaderModule MainModule { get; private set; }

    public static void Init(ILogger logger, ISettingsProvider settings)
    {
        Logger = logger;
        Settings = settings;
    }

    public static void Start()
    {
        var versionChecker = new GitHubVersionChecker(RepositoryName);
        var updater = new Updater(RepositoryName);

        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0";

        if (currentVersion == "0")
        {
            Warn("CurrentVersion is 0, faulty module version?");
        }

        var hasLatestVersion = versionChecker.IsLatestVersionInstalled(currentVersion);

        if (!hasLatestVersion)
        {
            updater.ShowUpdateMessageBox();
        }
        else
        {
            Msg("[VersionCheck] Latest version installed");
        }

        WindowHelper.SetWindowForeground(WindowHelper.GetUnityGameHwnd());

        var playerLogManager = new PlayerLogManager(Settings);

        string logPath = Path.Join(Environment.GetEnvironmentVariable("USERPROFILE"), "Appdata", "LocalLow",
            "infiniteloop", "DesktopMate");

        string playerLog = Path.Join(logPath, "Player.log");
        string playerPrevLog = Path.Join(logPath, "Player-prev.log");

        playerLogManager.ClearLog(playerLog);
        playerLogManager.ClearLog(playerPrevLog);

        ClassInjector.RegisterTypeInIl2Cpp<VrmLoaderModule>();
        GameObject obj = new("CustomAvatarLoader");
        Object.DontDestroyOnLoad(obj);
        MainModule = obj.AddComponent<VrmLoaderModule>();
    }

    public static void Msg(string message)
    {
        Logger.LogMessage(message);
    }

    public static void Warn(string message)
    {
        Logger.LogWarning(message);
    }

    public static void Error(string message)
    {
        Logger.LogError(message);
    }
}