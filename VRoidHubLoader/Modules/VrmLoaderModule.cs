using System.Runtime.InteropServices;
using CustomAvatarLoader.Settings;

namespace CustomAvatarLoader.Modules;

using CustomAvatarLoader.Helpers;
using Il2Cpp;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.UI;
using ILogger = Logging.ILogger;

public class VrmLoaderModule : IModule
{
    private bool init;

    public VrmLoaderModule(ILogger logger, ISettingsProvider settingsProvider)
    {
        Logger = logger;
        SettingsProvider = settingsProvider;
        VrmLoader = new VrmLoader(Logger);
    }

    protected virtual ILogger Logger { get; }
    
    protected virtual ISettingsProvider SettingsProvider { get; }

    protected virtual VrmLoader VrmLoader { get; }

    protected virtual CharaData CharaData { get; set; }

    protected virtual RuntimeAnimatorController RuntimeAnimatorController { get; set; }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hwnd, String text, String caption, uint type);
    
    protected virtual AsyncHelper AsyncHelper { get; set; }

    public readonly string VrmFolderPath = MelonEnvironment.GameRootDirectory + @"\VRM";

    public void OnInitialize()
    {
        AsyncHelper = new AsyncHelper();

        if (!Directory.Exists(VrmFolderPath))
        {
            Directory.CreateDirectory(VrmFolderPath);
            Logger.Debug("[Chara Loader] VRM folder does not exist. Creating one...");
        }
    }

    public async void OnUpdate()
    {
        if (!init)
        {
            string vrmPath = SettingsProvider.Get("vrmPath", string.Empty);
            if (GameObject.Find("/CharactersRoot")?.transform?.GetChild(0) != null
                && !string.IsNullOrEmpty(vrmPath))
            {
                LoadCharacter(vrmPath);
            }

            GenerateButtons();

            init = true;
        }

        AsyncHelper.OnUpdate();

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Logger.Debug("OnUpdate: VrmLoaderModule F4 pressed");

            // MenuManager is a singleton? sweet.
            if (!MenuManager.Instance.IsOpen)
            {
                MenuManager.Instance.OpenRootPage();
                MenuManager.Instance.OpenPage(MenuManager.Instance.modelPage);
            }
        }
    }

    // this should be put into a harmony patch at some point
    private void GenerateButtons()
    {
        GameObject mikuButton = GameObject.Find("Content/mikuButton");
        float offset = 0.32f;
        foreach (string path in Directory.GetFiles(VrmFolderPath).Where(f => f.EndsWith(".vrm")))
        {
            string file = Path.GetFileName(path);
            string name = file.Split('.')[0];
            GameObject button = DefaultControls.CreateButton(new DefaultControls.Resources());
            button.transform.position = new Vector3(0.83f, offset, -1f);
            button.transform.localScale = new Vector3(1.5f, 1.2f, 1f);
            button.name = name + "_button";
            button.GetComponentInChildren<Text>().text = name;
            button.GetComponent<Button>().onClick.AddListener(new Action(() =>
            {
                if (LoadCharacter(path))
                {
                    SettingsProvider.Set("vrmPath", path);
                    MelonPreferences.Save();

                    Logger.Debug("OnUpdate: VrmLoaderModule file chosen");
                }
            }));
            button.transform.SetParent(mikuButton.transform.parent.transform);
            Logger.Info("Loaded VRM " + file);
            offset -= 0.32f;
        }
        Logger.Debug("[Chara Loader] Custom menu buttons generated");
    }

    public bool LoadCharacter(string path)
    {
        if (!File.Exists(path))
        {
            Logger.Error("[Chara Loader] VRM file does not exist: " + path);
            
            return false;
        }

        var root = GameObject.Find("/CharactersRoot");
        var chara = root.transform.GetChild(0).gameObject;
        CharaData = chara.GetComponent<CharaData>();
        RuntimeAnimatorController = chara.GetComponent<Animator>().runtimeAnimatorController;

        Logger.Debug("Character attributes have been copied!");

        GameObject newChara = VrmLoader.LoadVrmIntoScene(path);
        if (newChara == null)
        {
            Logger.Error("[Chara Loader] Failed to load VRM file: " + path);
            Task.Run(() => { MessageBox(new IntPtr(0), "Failed to load VRM file! Make sure the VRM file is compatible!", "Error", 0x00000010 /* MB_ICONERROR */); });

            return false;
        }
        
        Logger.Debug("Old character has been destroyed.");
        Object.Destroy(chara);

        newChara.transform.parent = root.transform;

        CharaData newCharaData = newChara.AddComponent<CharaData>();
        CopyCharaData(CharaData, newCharaData);

        MainManager manager = GameObject.Find("MainManager").GetComponent<MainManager>();
        manager.charaData = newCharaData;

        Animator charaAnimator = newChara.GetComponent<Animator>();
        charaAnimator.applyRootMotion = true;
        charaAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        charaAnimator.runtimeAnimatorController = RuntimeAnimatorController;

        Logger.Debug("Character attribute replacement succeeded!");

        return true;
    }

    private void CopyCharaData(CharaData source, CharaData target)
    {
        target.alarmAnim = source.alarmAnim;
        target.draggedAnims = source.draggedAnims;
        target.hideLeftAnims = source.hideLeftAnims;
        target.hideRightAnims = source.hideRightAnims;
        target.jumpInAnim = source.jumpInAnim;
        target.jumpOutAnim = source.jumpOutAnim;
        target.pickedSittingAnim = source.pickedSittingAnim;
        target.pickedStandingAnim = source.pickedStandingAnim;
        target.sittingOneShotAnims = source.sittingOneShotAnims;
        target.sittingRandomAnims = source.sittingRandomAnims;
        target.standingOneShotAnims = source.standingOneShotAnims;
        target.standingRandomAnims = source.standingRandomAnims;
        target.strokedSittingAnim = source.strokedSittingAnim;
        target.strokedStandingAnim = source.strokedStandingAnim;
    }
}