using CustomAvatarLoader.Settings;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine.UI;
using UnityEngine;
using CustomAvatarLoader.Modules;

namespace CustomAvatarLoader.Patches
{
    [HarmonyPatch(typeof(ModelPageManager), "Start")]
    internal class ModelPageManagerPatch
    {
        protected static ISettingsProvider SettingsProvider { get; private set; }
        protected static Logging.ILogger Logger { get; private set; }
        protected static VrmLoaderModule LoaderModule { get; private set; }

        public static void InitPatch(Logging.ILogger logger, ISettingsProvider provider, VrmLoaderModule module)
        {
            SettingsProvider = provider;
            Logger = logger;
            LoaderModule = module;
        }

        private static void Postfix(ModelPageManager __instance)
        {
            float offset = 0.32f;
            foreach (string path in Directory.GetFiles(LoaderModule.VrmFolderPath).Where(f => f.EndsWith(".vrm")))
            {
                string file = Path.GetFileName(path);
                string name = file.Split('.')[0];
                GameObject button = DefaultControls.CreateButton(new DefaultControls.Resources());
                button.transform.position = new Vector3(0.83f, offset, -1f);
                button.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                button.name = name + "_button";
                button.GetComponentInChildren<Text>().text = name;
                button.GetComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    if (LoaderModule.LoadCharacter(path))
                    {
                        SettingsProvider.Set("vrmPath", path);
                        MelonPreferences.Save();

                        Logger.Debug("OnUpdate: VrmLoaderModule file chosen");
                    }
                }));
                button.transform.SetParent(__instance.mikuButton.transform.parent.transform, false);
                Logger.Info("Loaded VRM " + file);
                offset -= 0.32f;
            }
            Logger.Debug("[Chara Loader] Custom menu buttons generated");
        }
    }
}
