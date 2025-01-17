using System.Collections;
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
        private static FileSystemWatcher vrmWatcher;
        private static ModelPageManager instance;
        private static object? debounceCoroutine;
        
        public static void InitPatch(Logging.ILogger logger, ISettingsProvider provider, VrmLoaderModule module)
        {
            SettingsProvider = provider;
            Logger = logger;
            LoaderModule = module;
            
            InitFileSystemWatcher();
        }
        
        private static void InitFileSystemWatcher()
        {
            string vrmFolderPath = LoaderModule.VrmFolderPath;
            vrmWatcher = new FileSystemWatcher(vrmFolderPath, "*.vrm");
            vrmWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            vrmWatcher.Created += OnVrmFolderChanged;
            vrmWatcher.Deleted += OnVrmFolderChanged;
            vrmWatcher.EnableRaisingEvents = true;
        }

        private static void OnVrmFolderChanged(object sender, FileSystemEventArgs e)
        {
            Logger.Debug($"VRM file change detected: {e.ChangeType} - {e.FullPath}");
    
            if (debounceCoroutine != null)
            {
                MelonCoroutines.Stop(debounceCoroutine);
            }

            debounceCoroutine = MelonCoroutines.Start(DebounceCoroutine());
        }
        
        private static IEnumerator DebounceCoroutine()
        {
            yield return new WaitForSeconds(3f);

            RebuildButtons();

            debounceCoroutine = null;
        }
        
        private static void Postfix(ModelPageManager __instance)
        {
            instance = __instance;
            RebuildButtons();
        }

        private static void RebuildButtons()
        {
            if (instance == null) return;

            MelonCoroutines.Start(RebuildButtonsCoroutine());
        }

        private static IEnumerator RebuildButtonsCoroutine()
        {
            // Delete the old buttons
            Transform buttonParent = instance.mikuButton.transform.parent;
            foreach (Transform child in buttonParent.GetComponentsInChildren<Transform>())
            {
                if (child.name.EndsWith("_button"))
                {
                    UnityEngine.Object.Destroy(child.gameObject);
                }
            }

            // Regenerate the buttons
            float offset = 0.32f;
            foreach (string path in Directory.GetFiles(LoaderModule.VrmFolderPath).Where(f => f.EndsWith(".vrm")))
            {
                string file = Path.GetFileName(path);
                string name = file.Split('.')[0];
                GameObject button = DefaultControls.CreateButton(new DefaultControls.Resources());
                button.transform.position = new Vector3(0.83f, offset, -1f);
                RectTransform mikuButtonRect = instance.mikuButton.GetComponent<RectTransform>();
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                buttonRect.sizeDelta = mikuButtonRect.sizeDelta;
                button.name = name + "_button";
                Text buttonText = button.GetComponentInChildren<Text>();
                buttonText.text = name;
                buttonText.fontSize = 20;
                RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
                buttonTextRect.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                button.GetComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    if (LoaderModule.LoadCharacter(path))
                    {
                        SettingsProvider.Set("vrmPath", path);
                        MelonPreferences.Save();

                        Logger.Debug("OnUpdate: VrmLoaderModule file chosen");
                    }
                }));
                button.transform.SetParent(instance.mikuButton.transform.parent.transform, false);
                Logger.Info("Loaded VRM " + file);
                offset -= 0.32f;
                yield return null;
            }

            Logger.Debug("[Chara Loader] Custom menu buttons generated");
        }
    }
}