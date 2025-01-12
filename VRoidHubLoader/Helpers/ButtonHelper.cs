using ILogger = CustomAvatarLoader.Logging.ILogger;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;


namespace CustomAvatarLoader.Helpers

{
    internal class ButtonHelper
    {
        ILogger _logger;

        public ButtonHelper(ILogger logger)
        {
            _logger = logger;
        }

        public bool AddButton(Transform parent, Action action, string text)
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

            _logger.Info($"Button [{text}]: Has been added.");

            return true;
        }

        public void RemoveCustomButtons(Transform parent)
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
                        _logger.Info($"Button [{text}]: Has been removed.");
                    }
                }
            }
        }
    }
}
