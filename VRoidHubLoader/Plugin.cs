using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using UnityEngine;

namespace CustomAvatarLoader;

public class MelonLoaderPlugin : MelonMod
{
    public override void OnInitializeMelon()
    {
        LoggerInstance.Warning("Loading using MelonLoader!");

        ClassInjector.RegisterTypeInIl2Cpp<Core>();
        GameObject hook = new();
        hook.AddComponent<Core>().InitMelonLoader(LoggerInstance);
    }
}

[BepInPlugin("Custom Avatar Loader Mod", "Custom Avatar Loader Mod", "1.0.5")]
public class BepInExPlugin : BasePlugin
{
    public override void Load()
    {
        Log.LogWarning("Loading using BepInEx!");
       
        AddComponent<Core>().InitBepInEx(Log, Config);
    }
}
