using CustomAvatarLoader.Chara;
using CustomAvatarLoader.Helpers;

namespace CustomAvatarLoader;

using Logging;
using Versioning;
using MelonLoader;
using UnityEngine;
using System.Reflection;

public class Core : MelonMod
{
    private const string RepositoryName = "YusufOzmen01/desktopmate-custom-avatar-loader";
    private bool _init;

    protected virtual IServiceProvider Services { get; }

    private Logging.ILogger Logger { get; set; }

    private GitHubVersionChecker VersionChecker { get; set; }

    private Updater Updater { get; set; }

    private FileHelper FileHelper { get; set; }
    private VrmLoader VrmLoader { get; set; }

    private CharaLoader CharaLoader { get; set; }

    private string CurrentVersion { get; set; }

    private MelonPreferences_Category Settings { get; set; }

    private MelonPreferences_Entry<string> VrmPath { get; set; }

    public override void OnInitializeMelon()
    {
        CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0";

        Logger = new MelonLoaderLogger(LoggerInstance);
        VersionChecker = new GitHubVersionChecker(RepositoryName, Logger);
        Updater = new Updater(RepositoryName, Logger);
        FileHelper = new FileHelper();
        VrmLoader = new VrmLoader(Logger);
        CharaLoader = new CharaLoader(Logger, VrmLoader);

        if (CurrentVersion == "0")
            Logger.Warn("CurrentVersion is 0, faulty module version?");

        // Initialize your preferences
        Settings = MelonPreferences.CreateCategory("settings");
        VrmPath = Settings.CreateEntry("vrmPath", "");

        // Remove the contents of the log file and set it as readonly
        string logPath = Path.Join(Environment.GetEnvironmentVariable("USERPROFILE"), "Appdata", "LocalLow", "infiniteloop", "DesktopMate");

        string playerLog = Path.Join(logPath, "Player.log");
        string playerPrevLog = Path.Join(logPath, "Player-prev.log");

        if (File.Exists(playerLog))
        {
            File.SetAttributes(playerLog, FileAttributes.Normal);
            try { File.WriteAllText(playerLog, string.Empty); } catch { /* empty */ }
            File.SetAttributes(playerLog, FileAttributes.ReadOnly);
        }

        if (File.Exists(playerPrevLog))
        {
            File.SetAttributes(playerPrevLog, FileAttributes.Normal);
            try { File.WriteAllText(playerPrevLog, string.Empty); } catch { /* empty */ }
            File.SetAttributes(playerPrevLog, FileAttributes.ReadOnly);
        }

        var hasLatestVersion = VersionChecker.IsLatestVersionInstalled(CurrentVersion);

        if (!hasLatestVersion)
        {
            Updater.ShowUpdateMessageBox();
        }
        else
        {
            Logger.Info("[VersionCheck] Latest version installed");
        }
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            string path = FileHelper.OpenFileDialog();
            if (!string.IsNullOrEmpty(path) && CharaLoader.LoadCharacter(path))
            {
                VrmPath.Value = path;
                _init = true;
                MelonPreferences.Save();
            }
        }

        if (!_init && GameObject.Find("/CharactersRoot").transform.GetChild(0) != null)
        {
            _init = true;
            if (VrmPath.Value != "") CharaLoader.LoadCharacter(VrmPath.Value);
        }

        if (!_init || GameObject.Find("/CharactersRoot/VRMFILE") != null || VrmPath.Value == "")
            return;

        VrmPath.Value = "";
        MelonPreferences.Save();
    }
}