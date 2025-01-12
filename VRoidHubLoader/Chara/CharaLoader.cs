using CustomAvatarLoader.Helpers;
using CustomAvatarLoader.Modules;
using Il2Cpp;
using Il2CppUniGLTF;
using Il2CppUniVRM10;
using UnityEngine;
using ILogger = CustomAvatarLoader.Logging.ILogger;

namespace CustomAvatarLoader.Chara;

public class CharaLoader
{
    private ILogger _logger;
    private VrmLoader _vrmLoader;
    private CharaData CharaData { get; set; }
    private RuntimeAnimatorController RuntimeAnimatorController { get; set; }

    public IList<IModule> Modules { get; } = new List<IModule>();

    public CharaLoader(ILogger logger, VrmLoader vrmLoader)
    {
        _logger = logger;
        _vrmLoader = vrmLoader;
    }
    
    public bool LoadCharacter(string path)
    {
        if (!File.Exists(path))
        {
            _logger.Error("[Chara Loader] VRM file does not exist: " + path);
            return false;
        }

        var chara = GameObject.Find("/CharactersRoot").transform.GetChild(0).gameObject;
        CharaData = chara.GetComponent<CharaData>();
        RuntimeAnimatorController = chara.GetComponent<Animator>().runtimeAnimatorController;

        _logger.Debug("Character attributes have been copied. Removing existing character...");
        UnityEngine.Object.Destroy(chara);

        GameObject newChara = _vrmLoader.LoadVrmIntoScene(path);
        if (newChara == null)
        {
            _logger.Error("[Chara Loader] Failed to load VRM file: " + path);
            
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

        _logger.Debug("Character attribute replacement succeeded!");

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