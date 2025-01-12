using CustomAvatarLoader.Chara;
using CustomAvatarLoader.Helpers;

namespace CustomAvatarLoader;

using Logging;
using Modules;
using Versioning;
using Il2Cpp;
using Il2CppTMPro;
using Il2CppUniGLTF;
using Il2CppUniVRM10;
using MelonLoader;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Reflection;

public class Core : MelonMod
{
    private const string RepositoryName = "YusufOzmen01/desktopmate-custom-avatar-loader";
    private bool _init;
    private bool _initOpenModelButton;

    protected virtual IServiceProvider Services { get; }
    
    private Logging.ILogger Logger { get; set; }

    private GitHubVersionChecker VersionChecker { get; set; }
    
    private Updater Updater { get; set; }
    
    private FileHelper FileHelper { get; set; }
    private VrmLoader VrmLoader { get; set; }
    private ButtonHelper ButtonHelper { get; set; }
    
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
        ButtonHelper = new ButtonHelper(Logger);
        CharaLoader = new CharaLoader(Logger, VrmLoader);

        if (CurrentVersion == "0")
            Logger.Warn("CurrentVersion is 0, faulty module version?");
        
        // Initialize your preferences
        Settings = MelonPreferences.CreateCategory("settings");
        VrmPath = Settings.CreateEntry("vrmPath", "");

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

        if (!_initOpenModelButton && GameObject.Find("iltanButton") is GameObject iltanButton)
        {
            _initOpenModelButton = true;
            Action actionChoose = new Action(() =>
            {
                string path = FileHelper.OpenFileDialog();
                if (!string.IsNullOrEmpty(path) && CharaLoader.LoadCharacter(path))
                {
                    VrmPath.Value = path;
                    _init = true;
                    MelonPreferences.Save();
                }
            });
            ButtonHelper.AddButton(iltanButton.transform.parent, actionChoose, "Open...");
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