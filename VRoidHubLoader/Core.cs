using CustomAvatarLoader;
using Il2Cpp;
using Il2CppTMPro;
using Il2CppUniGLTF;
using Il2CppUniVRM10;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(Core), "Custom Avatar Loader Mod", "1.0.3", "SergioMarquina, Misandrie, MrKoteee")]
[assembly: MelonGame("infiniteloop", "DesktopMate")]

namespace CustomAvatarLoader;

public class Core : MelonMod
{
    CharaData _charaData;
    RuntimeAnimatorController _runtimeAnimatorController;
    private MelonPreferences_Category _settings;
    private MelonPreferences_Entry<string> _vrmPath;

    public override void OnInitializeMelon()
    {
        _settings = MelonPreferences.CreateCategory("settings");
        _vrmPath = _settings.CreateEntry("vrmPath", "");
    }

    private bool _init = false;
    private bool _init_buttons = false;

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ShowChooseModelWindow();
        }

        // Add Buttons
        if (!_init_buttons && GameObject.Find("iltanButton") is GameObject iltanButton)
        {
            _init_buttons = true;
            Action actionChoose = new Action(() => ShowChooseModelWindow());
            AddButton(iltanButton.transform.parent, actionChoose, "Open...");
            
        }
        // End code add Buttons

        if (!_init && GameObject.Find("/CharactersRoot").transform.GetChild(0) != null)
        {
            _init = true;
            if (_vrmPath.Value != "") LoadCharacter(_vrmPath.Value);
        }

        if (!_init || GameObject.Find("/CharactersRoot/VRMFILE") != null || _vrmPath.Value == "") 
            return;
        
        _vrmPath.Value = "";
        MelonPreferences.Save();
    }

    private void ShowChooseModelWindow()
    {
        string path = FileHelper.OpenFileDialog();
        if (!string.IsNullOrEmpty(path) && LoadCharacter(path))
        {
            _vrmPath.Value = path;
            _init = true;
            MelonPreferences.Save();
        }
    }

    private void RemoveCustomButtons(Transform parent)
    {
        var contentButtons = parent.gameObject;

        for (int i = 0; i < contentButtons.transform.childCount; ++i)
        {
            var button = contentButtons.transform.GetChild(i).gameObject;
            if (button.transform.childCount >= 0)
            {
                var textField = button.transform.GetChild(0).gameObject;
                var textMeshComp = textField.GetComponent<TextMeshProUGUI>();
                var text = textMeshComp.text;

                if (textField.name.InvariantEqualsIgnoreCase("Text (Custom)"))
                {
                    button.active = false;
                    UnityEngine.Object.Destroy(button);
                    LoggerInstance.Msg($"Button [{text}]: Has been removed.");
                }
            }
        }
    }

    private bool AddButton(Transform parent, Action action, string text)
    {
        var baseButton = GameObject.Find("MenuCanvas/MenuParent/RootPage/Scroll View/Viewport/Content/ModelButton");
        if (baseButton == null) return false;

        GameObject newButton = GameObject.Instantiate(baseButton, parent);
        Image newButtonImage = newButton.GetComponent<Image>();
        newButtonImage.sprite = null;

        Button newButtonComp = newButton.GetComponent<Button>();
        newButtonComp.onClick.RemoveAllListeners();
        newButtonComp.onClick.AddListener(action);

        var textField = newButton.transform.FindChild("Text (TMP)").gameObject;
        textField.name = "Text (Custom)";
        textField.active = true;

        var textMeshComp = textField.GetComponent<TextMeshProUGUI>();
        textMeshComp.text = text;
        textMeshComp.GenerateTextMesh();

        LoggerInstance.Msg($"Button [{text}]: Has been added.");

        return true;
    }

    private bool LoadCharacter(string path)
    {
        if (!File.Exists(path))
        {
            LoggerInstance.Error("VRM file does not exist: " + path);
            return false;
        }

        GameObject newChara;
        try
        {
            var data = new GlbFileParser(path).Parse();
            var vrmdata = Vrm10Data.Parse(data);
            if (vrmdata == null)
            {
                MigrationData migrationData;
                Vrm10Data.Migrate(data, out vrmdata, out migrationData);
                if (vrmdata == null)
                {
                    throw new System.Exception("Cannot load vrm file!");
                }
            }

            var context = new Vrm10Importer(vrmdata);
            var loaded = context.Load();

            loaded.EnableUpdateWhenOffscreen();
            loaded.ShowMeshes();
            loaded.gameObject.name = "VRMFILE";
            newChara = loaded.gameObject;
        }
        catch (System.Exception e)
        {
            LoggerInstance.Error("Error trying to load the VRM file! : " + e.Message);
            return false;
        }

        var chara = GameObject.Find("/CharactersRoot").transform.GetChild(0).gameObject;
        _charaData = chara.GetComponent<CharaData>();
        _runtimeAnimatorController = chara.GetComponent<Animator>().runtimeAnimatorController;

        LoggerInstance.Msg("Chara copied! Removing default chara...");
        UnityEngine.Object.Destroy(chara);

        newChara.transform.parent = GameObject.Find("/CharactersRoot").transform;

        CharaData newCharaData = newChara.AddComponent<CharaData>();
        CopyCharaData(_charaData, newCharaData);

        MainManager manager = GameObject.Find("MainManager").GetComponent<MainManager>();
        manager.charaData = newCharaData;

        Animator charaAnimator = newChara.GetComponent<Animator>();
        charaAnimator.applyRootMotion = true;
        charaAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        charaAnimator.runtimeAnimatorController = _runtimeAnimatorController;

        LoggerInstance.Msg("Chara replaced!");

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