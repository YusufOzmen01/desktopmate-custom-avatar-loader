using CustomAvatarLoader;
using CustomAvatarLoader.Settings;


#if MELON
using MelonLoader;
[assembly: MelonInfo(typeof(MelonLoaderPlugin), "Custom Avatar Loader Mod", "1.0.5", "SergioMarquina, Misandrie, CrasH, alltoasters")]
[assembly: MelonGame("infiniteloop", "DesktopMate")]
#endif
#if BEPINEX
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
#endif

namespace CustomAvatarLoader;

#if MELON
public class MelonLoaderPlugin : MelonMod, Logging.ILogger
{
    public Action<object> LogMessage => LoggerInstance.Msg;

    public Action<object> LogWarning => LoggerInstance.Warning;

    public Action<object> LogError => LoggerInstance.Error;

    public override void OnLateInitializeMelon()
    {
        LoggerInstance.Warning("Loading using MelonLoader!");
        Core.Init(this, new MelonLoaderSettings("settings"));
    }
}
#endif

#if BEPINEX
[BepInPlugin("CustomAvatarLoaderMod", "Custom Avatar Loader Mod", "1.0.5")]
public class BepInExPlugin : BasePlugin, Logging.ILogger
{
    public static BepInExPlugin Instance;

    public Harmony Harmony;

    public Action<object> LogMessage => Log.LogMessage;

    public Action<object> LogWarning => Log.LogWarning;

    public Action<object> LogError => Log.LogError;

    public override void Load()
    {
        Log.LogWarning("Loading using BepInEx!");
        Instance = this;
        Harmony = new Harmony("CustomAvatarLoaderMod");
        Harmony.PatchAll();
        Core.Init(this, new BepInExSettings("settings"));
    }
}
#endif