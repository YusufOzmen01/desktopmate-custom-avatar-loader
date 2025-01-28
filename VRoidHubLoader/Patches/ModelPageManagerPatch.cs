using HarmonyLib;
using UnityEngine.UI;
using UnityEngine;
#if MELON
using Il2Cpp;
#endif

namespace CustomAvatarLoader.Patches
{
    [HarmonyPatch(typeof(ModelPageManager), "Start")]
    internal class ModelPageManagerPatch
    {
        private static void Postfix(ModelPageManager __instance)
        {
            float offset = 0.32f;
            foreach (string path in Directory.GetFiles(Core.MainModule.VrmFolderPath).Where(f => f.EndsWith(".vrm")))
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
                    if (Core.MainModule.LoadCharacter(path))
                    {
                        Core.Settings.Set("vrmPath", path);
                        Core.Settings.SaveSettings();

                        Core.Msg("OnUpdate: VrmLoaderModule file chosen");
                    }
                }));
                button.transform.SetParent(__instance.mikuButton.transform.parent.transform, false);
                Core.Msg("Loaded VRM " + file);
                offset -= 0.32f;
            }
            Core.Msg("[Chara Loader] Custom menu buttons generated");
        }
    }
}
