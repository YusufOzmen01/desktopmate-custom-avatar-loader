using HarmonyLib;
using UnityEngine.UI;
using UnityEngine;
#if MELON
using Il2Cpp;
#endif

namespace CustomAvatarLoader.Patches
{
    [HarmonyPatch(typeof(ModelPageManager), "Start")]
    internal static class ModelPageManagerPatch
    {
        private static ModelPageManager MPM;

        private readonly static List<GameObject> OurButtons = [];

        private static void Postfix(ModelPageManager __instance)
        {
            MPM = __instance;

            GameObject import = DefaultControls.CreateButton(new DefaultControls.Resources());
            import.transform.position = new Vector3(0.83f, 0.3275f, -1f);
            import.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            import.name = "importButton";
            import.GetComponentInChildren<Text>().text = "Import...";
            import.GetComponent<Button>().onClick.AddListener(new Action(async () =>
            {
                await Core.MainModule.ImportVRM();
            }));
            import.transform.SetParent(MPM.mikuButton.transform.parent.transform, false);
            SpawnButtons();
        }

        public static void SpawnButtons()
        { 
            foreach (GameObject button in OurButtons)
            {
                UnityEngine.Object.Destroy(button);
            }

            OurButtons.Clear();
            float offset = 0.07f;
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

                        Core.Msg("Button: VrmLoaderModule file chosen");
                    }
                }));
                button.transform.SetParent(MPM.mikuButton.transform.parent.transform, false);
                OurButtons.Add(button);
                Core.Msg("Loaded VRM " + file);
                offset -= 0.25f;
            }
            Core.Msg("[Chara Loader] Custom menu buttons generated");
        }
    }
}
