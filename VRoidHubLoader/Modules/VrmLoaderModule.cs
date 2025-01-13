namespace CustomAvatarLoader.Modules;

using CustomAvatarLoader.Helpers;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using ILogger = Logging.ILogger;

public class VrmLoaderModule : IModule
{
    private bool init;

    public VrmLoaderModule(ILogger logger)
    {
        Logger = logger;
        VrmLoader = new VrmLoader(Logger);
    }

    protected virtual ILogger Logger { get; }

    protected virtual VrmLoader VrmLoader { get; }

    protected virtual CharaData CharaData { get; set; }

    protected virtual RuntimeAnimatorController RuntimeAnimatorController { get; set; }

    protected virtual MelonPreferences_Entry<string> VrmPath { get; set; }

    protected virtual AsyncHelper AsyncHelper { get; set; }

    public void OnInitialize()
    {
        var settings = MelonPreferences.CreateCategory("settings");
        VrmPath = settings.CreateEntry("vrmPath", "");
        AsyncHelper = new AsyncHelper();
    }

    public async void OnUpdate()
    {
        if (!init)
        {
            if (GameObject.Find("/CharactersRoot")?.transform?.GetChild(0) != null
                && VrmPath.Value != string.Empty)
            {
                LoadCharacter(VrmPath.Value);
            }

            init = true;
        }

        AsyncHelper.OnUpdate();

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Logger.Debug($"OnUpdate: VrmLoaderModule F4 pressed");

            var fileHelper = new FileHelper();

            string path = await fileHelper.OpenFileDialog();

            AsyncHelper.RunOnMainThread(() => {
                if (!string.IsNullOrEmpty(path) && LoadCharacter(path))
                {
                    VrmPath.Value = path;
                    MelonPreferences.Save();

                    Logger.Debug($"OnUpdate: VrmLoaderModule file chosen");
                }
            });
        }
    }

    public bool LoadCharacter(string path)
    {
        if (!File.Exists(path))
        {
            Logger.Error("[Chara Loader] VRM file does not exist: " + path);
            return false;
        }

        var chara = GameObject.Find("/CharactersRoot").transform.GetChild(0).gameObject;
        CharaData = chara.GetComponent<CharaData>();
        RuntimeAnimatorController = chara.GetComponent<Animator>().runtimeAnimatorController;

        Logger.Debug("Character attributes have been copied. Removing existing character...");
        Object.Destroy(chara);

        GameObject newChara = VrmLoader.LoadVrmIntoScene(path);
        if (newChara == null)
        {
            Logger.Error("[Chara Loader] Failed to load VRM file: " + path);

            return false;
        }

        newChara.transform.parent = GameObject.Find("/CharactersRoot").transform;

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