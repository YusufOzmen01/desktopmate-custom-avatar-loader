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
using CustomAvatarLoader.Patches;
#endif

public class VrmLoaderModule : MonoBehaviour
{
    private bool init;

    private VrmLoader VrmLoader;

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hwnd, String text, String caption, uint type);

#if MELON
    public readonly string VrmFolderPath = MelonEnvironment.GameRootDirectory + @"\VRM";
#endif
#if BEPINEX
    public readonly string VrmFolderPath = BepInEx.Paths.GameRootPath + @"\VRM";
#endif

    private string? ModelToApply = null;

    private void Awake()
    {
        if (!Directory.Exists(VrmFolderPath))
        {
            Directory.CreateDirectory(VrmFolderPath);
            Core.Warn("[Chara Loader] VRM folder does not exist. Creating one...");
        }

        VrmLoader = new VrmLoader();
    }

    private async void Update()
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

        // After selecting a model via F4/import, we have to wait a frame to actually apply the model
        // Attempting to do so inside of ImportVRM() causes a AccessViolationException
        if (ModelToApply != null)
        {
            if (LoadCharacter(ModelToApply))
            {
                Core.Settings.Set("vrmPath", ModelToApply);
                Core.Settings.SaveSettings();

                Core.Msg("Update: Model file chosen");
                ModelToApply = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Core.Msg("OnUpdate: VrmLoaderModule F4 pressed");

            await ImportVRM();
        }
    }

    [HideFromIl2Cpp]
    public async Task ImportVRM()
    {
        string? path = await Core.FileHelper.OpenFileDialog();

        if (!string.IsNullOrEmpty(path))
        {
            string fileName = Path.GetFileName(path);
            string destination = VrmFolderPath + '\\' + fileName;
            if (File.Exists(destination))
            {
                // TODO: allow the user to decide if the old file should be overwritten
                Core.Warn("Duplicate model file detected. The old model file will be overwritten!");
            }
            File.Copy(path, destination, true);
            ModelPageManagerPatch.SpawnButtons();
            Core.Msg($"Added {fileName} to VRM folder");
            ModelToApply = destination;
        }

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

        GameObject? newChara = VrmLoader.LoadVrmIntoScene(path);
        if (newChara == null)
        {
            Core.Error("[Chara Loader] Failed to load VRM file: " + path);
            Task.Run(() => { MessageBox(new IntPtr(0), "Failed to load VRM file! Make sure the VRM file is compatible!", "Error", 0x00000010 /* MB_ICONERROR */); });

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