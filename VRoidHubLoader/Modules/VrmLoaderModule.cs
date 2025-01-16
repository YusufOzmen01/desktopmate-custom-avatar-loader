using System.Runtime.InteropServices;

namespace CustomAvatarLoader.Modules;

using CustomAvatarLoader.Helpers;
using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using UnityEngine;

#if MELON
using Il2Cpp;
using MelonLoader;
using MelonLoader.Utils;
#endif

#if BEPINEX
using BepInEx.Unity.IL2CPP.Utils;
#endif

using AssetBundleLoader = Helpers.AssetBundleLoader;

public class VrmLoaderModule : MonoBehaviour
{
    private bool init;

    private VrmLoader? VrmLoader;
    private AssetBundleLoader? AssetBundleLoader;

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hwnd, String text, String caption, uint type);

#if MELON
    public readonly string VrmFolderPath = MelonEnvironment.GameRootDirectory + @"\VRM";
#endif
#if BEPINEX
    public readonly string VrmFolderPath = BepInEx.Paths.GameRootPath + @"\VRM";
#endif

    private void Awake()
    {
        if (!Directory.Exists(VrmFolderPath))
        {
            Directory.CreateDirectory(VrmFolderPath);
            Core.Warn("[Chara Loader] VRM folder does not exist. Creating one...");
        }

        VrmLoader = new VrmLoader();
        AssetBundleLoader = new AssetBundleLoader();
    }

    private void Update()
    {
        if (!init)
        {
            string vrmPath = Core.Settings.Get("vrmPath", string.Empty);
            if (GameObject.Find("/CharactersRoot")?.transform?.GetChild(0) != null
                && !string.IsNullOrEmpty(vrmPath))
            {
                LoadCharacter(vrmPath);
                
                init = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Core.Msg("OnUpdate: VrmLoaderModule F4 pressed");

            // MenuManager is a singleton? sweet.
            if (!MenuManager.Instance.IsOpen)
            {
                #if MELON
                MelonCoroutines.Start(CoAutoOpenModelPage());
                #endif
                #if BEPINEX
                MonoBehaviourExtensions.StartCoroutine(this, CoAutoOpenModelPage());
                #endif
            }
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator CoAutoOpenModelPage()
    {
        MenuManager.Instance.OpenRootPage();
        while (MenuManager.Instance.isMoving)
            yield return null;
        MenuManager.Instance.OpenPage(MenuManager.Instance.modelPage);
    }

    public bool LoadCharacter(string path)
    {
        if (!File.Exists(path))
        {
            Core.Error("[Chara Loader] VRM file does not exist: " + path);
            
            return false;
        }

        var root = GameObject.Find("/CharactersRoot");
        var chara = root.transform.GetChild(0).gameObject;
        var data = chara.GetComponent<CharaData>();
        var controller = chara.GetComponent<Animator>().runtimeAnimatorController;

        Core.Msg("Character attributes have been copied!");

        GameObject? newChara;
        AssetBundle.UnloadAllAssetBundles(false);
        if (path.EndsWith(".vrm")) newChara = VrmLoader!.LoadVrmIntoScene(path);
        else if (path.EndsWith(".dmma")) newChara = AssetBundleLoader!.LoadAssetBundleIntoScene(path);
        else
        {
            Core.Error("Unkown file extension of selected model file: " + path);
            return false;
        }
        if (newChara == null)
        {
            Core.Error("[Chara Loader] Failed to load model from file: " + path);
            Task.Run(() => { MessageBox(new IntPtr(0), "Failed to load model from file! Make sure the file is VRM or DMMA compatible!", "Error", 0x00000010 /* MB_ICONERROR */); });

            return false;
        }
        
        Core.Msg("Old character has been destroyed.");
        Object.Destroy(chara);

        newChara.transform.parent = root.transform;

        CharaData newCharaData = newChara.AddComponent<CharaData>();
        CopyCharaData(data, newCharaData);

        MainManager manager = GameObject.Find("MainManager").GetComponent<MainManager>();
        manager.charaData = newCharaData;

        Animator charaAnimator = newChara.GetComponent<Animator>();
        charaAnimator.applyRootMotion = true;
        charaAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        charaAnimator.runtimeAnimatorController = controller;

        Core.Msg("Character attribute replacement succeeded!");

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