﻿using HarmonyLib;
#if MELON
using Il2Cpp;
#endif

namespace CustomAvatarLoader.Patches
{
    [HarmonyPatch(typeof(MainManager), "Start")]
    internal class MainManagerPatch
    {
        // BepInEx's Load() function fires too early, and the hook GameObject generated by Core doesn't spawn
        // therefore, we leech off of something else early on in the startup process to do our own setup
        private static void Prefix()
        {
            Core.Start();
        }
    }
}
