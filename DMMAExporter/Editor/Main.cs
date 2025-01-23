#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using UnityEngine.UIElements;

namespace DMMAExporter
{
    public class DMMAExporterWindow : EditorWindow
    {
        [MenuItem("Tools/DMMA Exporter")]
        public static void ShowExample()
        {
            var wnd = GetWindow<DMMAExporterWindow>();
            wnd.titleContent = new GUIContent("DMMA Exporter");
        }

        private const int SMALL_TITLE_WIDTH = 345;

        public List<DesktopMateModdedAvatar> avatars = new();
        public List<Error> errors = new();

        public ListView? avatarList;
        public ListView? errorList;
        public Label? avatarNameLabel;
        public Label? noAvatarsFoundText;

        public Button? exportBtn;
        public Button? dryrunBtn;

        public Validator validator = new Validator();

        public void CreateGUI()
        {
            avatars = FindObjectsOfType<DesktopMateModdedAvatar>().ToList();
            avatars.Sort(new comparers.ComponentNameComparer());

            var root = rootVisualElement;

            var styleSheetPath = "Assets/DMMAExporter/Editor/Global.uss";
            var globalStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            root.styleSheets.Add(globalStyleSheet);

            // Title
            var label = new Label("DMMA Exporter");
            label.style.unityTextAlign = TextAnchor.UpperCenter;
            label.style.fontSize = 18;
            label.style.marginTop = 10;
            label.style.marginBottom = 10;
            label.RegisterCallback((GeometryChangedEvent evt) => {
                if (evt.newRect.width > SMALL_TITLE_WIDTH && evt.oldRect.width <= SMALL_TITLE_WIDTH)
                    label.text = "DesktopMate Modded Avatar Exporter";
                else if (evt.newRect.width <= SMALL_TITLE_WIDTH && evt.oldRect.width > SMALL_TITLE_WIDTH)
                    label.text = "DMMA Exporter";
            });
            root.Add(label);

            // Avatar List
            avatarList = new ListView();
            avatarList.selectionType = SelectionType.Single;
            avatarList.style.flexShrink = 0;
            avatarList.itemsSource = avatars;
            avatarList.makeItem = () => new Button();
            avatarList.bindItem = (e, i) => {
                var btn = (e as Button)!;
                btn.text = avatars[i].gameObject.name;
                btn.clicked += () => {
                    avatarList.SetSelection(i);
                };
            };
            avatarList.selectedIndicesChanged += (e) => {
                var indexes = e.ToArray();
                if (indexes.Length == 0) return;
                var newSelected = avatars[indexes[0]];
                avatarNameLabel!.text = newSelected.gameObject.name;
                RefreshErrorList();
            };
            root.Add(avatarList);

            noAvatarsFoundText = new Label();
            noAvatarsFoundText.text = "No models with DesktopMateModdedAvatar script found in scene";
            noAvatarsFoundText.style.unityTextAlign = TextAnchor.MiddleCenter;
            root.Add(noAvatarsFoundText);

            // Spacer
            var line = new VisualElement();
            line.style.height = 2;
            line.style.minHeight = 2;
            line.style.maxHeight = 2;
            line.style.borderTopWidth = 1;
            line.style.marginTop = 3;
            line.style.borderTopColor = Color.grey;
            root.Add(line);

            // Avatar Name
            avatarNameLabel = new Label(); 
            avatarNameLabel.style.unityTextAlign = TextAnchor.UpperCenter;
            avatarNameLabel.text = "No model selected";
            avatarNameLabel.style.fontSize = 14;
            avatarNameLabel.style.marginTop = 10;
            avatarNameLabel.style.marginBottom = 10;
            root.Add(avatarNameLabel);

            // Avatar Errors
            var errorHeight = 35;
            var errorPaddingX = 7;
            errorList = new ListView();
            errorList.selectionType = SelectionType.None;
            errorList.itemsSource = errors;
            errorList.fixedItemHeight = errorHeight;
            errorList.makeItem = () => {
                var container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;

                var icon = new VisualElement();
                icon.style.width = errorHeight;
                icon.style.height = errorHeight;
                icon.style.marginLeft = errorPaddingX;
                icon.style.marginRight = errorPaddingX;
                icon.style.flexGrow = 0;
                icon.style.flexShrink = 0;
                container.Add(icon);

                var label = new Label();
                label.style.whiteSpace = WhiteSpace.Normal;
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.style.marginRight = errorPaddingX;
                label.style.flexGrow = 1;
                label.style.flexShrink = 1;
                container.Add(label);

                return container;
            };
            errorList.bindItem = (e, i) => {
                var container = (e as VisualElement)!;
                var children = container.Children().ToArray();
                var icon = (children[0] as VisualElement)!;
                var label = (children[1] as Label)!;

                icon.RemoveFromClassList("error-icon");
                icon.RemoveFromClassList("warning-icon");
                icon.RemoveFromClassList("info-icon");
                icon.RemoveFromClassList("success-icon");

                var error = errors[i];

                if (error.type == ErrorType.Error) icon.AddToClassList("error-icon");
                if (error.type == ErrorType.Warning) icon.AddToClassList("warning-icon");
                if (error.type == ErrorType.Info) icon.AddToClassList("info-icon");
                if (error.type == ErrorType.Success) icon.AddToClassList("success-icon");

                label.text = error.msg;
            };
            root.Add(errorList);

            // Spacer
            var spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            spacer.style.flexShrink = 1;
            spacer.style.minHeight = 0;
            root.Add(spacer);

            // Bottom Buttons
            var height = 30;
            var bottomButtonBar = new VisualElement();
            bottomButtonBar.style.flexDirection = FlexDirection.Row;
            bottomButtonBar.style.flexGrow = 0;
            bottomButtonBar.style.height = height;
            bottomButtonBar.style.minHeight = height;
            bottomButtonBar.style.maxHeight = height;
            root.Add(bottomButtonBar);

            dryrunBtn = new Button();
            dryrunBtn.style.flexGrow = 1;
            dryrunBtn.name = "test";
            dryrunBtn.text = "Test";
            dryrunBtn.clicked += () => Exporter.Export(
                avatars[avatarList.selectedIndex].gameObject, true
            );
            bottomButtonBar.Add(dryrunBtn);

            exportBtn = new Button();
            exportBtn.style.flexGrow = 1;
            exportBtn.name = "export";
            exportBtn.text = "Export";
            exportBtn.clicked += () => Exporter.Export(
                avatars[avatarList.selectedIndex].gameObject, false
            );
            bottomButtonBar.Add(exportBtn);

            RefreshErrorList();
            rootVisualElement.schedule.Execute(RefreshAvatarList).Every(500);
            rootVisualElement.schedule.Execute(RefreshErrorList).Every(1000);
        }

        private void SetButtonsEnabled(bool enabled)
        {
            dryrunBtn!.SetEnabled(enabled);
            exportBtn!.SetEnabled(enabled);
        }

        private void ClearSelectedModel()
        {
            SetButtonsEnabled(false);
            avatarNameLabel!.text = "No model selected";
            errors.Clear();
            errorList!.Rebuild();
        }

        private void RefreshErrorList()
        {
            int? selected = avatarList!.selectedIndex;
            if (selected == null || !(0 <= selected && selected < avatars.Count))
            {
                return;
            }

            var selectedComponent = avatars[selected ?? -1];
            if (selectedComponent == null || selectedComponent.gameObject == null)
            {
                avatarList.SetSelection(-1);
                ClearSelectedModel();
                return;
            }

            errors.RemoveAll((e) => true);
            validator.ValidateSelectedModel(errors, selectedComponent.gameObject);

            var hasErrors = errors.Any((e) => e.type == ErrorType.Error);
            if (hasErrors)
            {
                SetButtonsEnabled(false);
            }
            else
            {
                errors.Add(new Error("No errors found", ErrorType.Success));
                SetButtonsEnabled(true);
            }

            errorList!.Rebuild();
        }

        private void RefreshAvatarList()
        {
            noAvatarsFoundText!.style.display = DisplayStyle.None;

            var foundAvatars = FindObjectsOfType<DesktopMateModdedAvatar>();
            Array.Sort(foundAvatars, new comparers.ComponentNameComparer());

            if (foundAvatars.Length != avatars.Count)
            {
                GameObject? previousObject = null;
                var selected = avatarList!.selectedIndex;
                if (0 <= selected && selected < avatars.Count)
                {
                    var selectedObject = avatars[selected];
                    if (selectedObject != null)
                        previousObject = selectedObject.gameObject;
                }

                avatars.RemoveAll((i) => true);
                avatars.AddRange(foundAvatars);

                if (previousObject == null)
                {
                    avatarList.selectedIndex = -1;
                }
                else
                {
                    var found = avatars.FindIndex((i) => i.gameObject == previousObject);
                    if (found == -1)
                        avatarList.selectedIndex = -1;
                    else
                        avatarList.selectedIndex = found;
                }

                avatarList.Rebuild();
            }
            else
            {
                var somethingDifferent = false;
                for (int i = 0; i < foundAvatars.Length; i++)
                {
                    if (avatars[i] != foundAvatars[i])
                    {
                        avatars[i] = foundAvatars[i];
                        somethingDifferent = true;
                    }
                }
                if (somethingDifferent)
                {
                    avatars.Sort(new comparers.ComponentNameComparer());
                    avatarList!.Rebuild();
                }
            }

            // Add text notifying the user that no avatars found in scene if that is appropriate.
            // This should really be done in a better place, but that is only available in Unity
            // 2023 and onwards: https://discussions.unity.com/t/listview-changing-list-is-empty-label/879109
            if (avatars.Count > 0)
                noAvatarsFoundText.style.display = DisplayStyle.None;
            else
                noAvatarsFoundText.style.display = DisplayStyle.Flex;
        }
    }
}