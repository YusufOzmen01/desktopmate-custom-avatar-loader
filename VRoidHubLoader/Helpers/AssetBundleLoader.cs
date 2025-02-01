namespace CustomAvatarLoader.Helpers;

using UnityEngine;

public class AssetBundleLoader
{
    public AssetBundleLoader()
    {
    }

    private void CleanPotentiallyDangerousGameObject(GameObject gameObject)
    {
        var components = gameObject.GetComponents<Component>();
        if (components != null)
        {
            foreach (var component in components)
            {
                if (component == null) continue;

                var fullName = component.GetIl2CppType().FullName;
                if (fullName == null)
                {
                    Core.Warn("Failed to get full name of component: " + component.ToString());
                    Object.Destroy(component);
                    continue;
                }

                if (!allowedComponentNames.Contains(fullName))
                {
                    Core.Warn($"Removing component \"{fullName}\" from game object {gameObject.name} as it is not in the list of safe components.");
                    Object.Destroy(component);
                    continue;
                }
            }
        }

        for (int i = 0; i < gameObject.transform.childCount; i++)
            CleanPotentiallyDangerousGameObject(gameObject.transform.GetChild(i).gameObject);
    }

    public GameObject? LoadAssetBundleIntoScene(string path)
    {
        Core.Msg($"Loading Asset Bundle Into Scene: \"{path}\"");

        var bundle = AssetBundle.LoadFromFile(path);
        if (bundle == null)
        {
            Core.Error("Failed to load asset bundle!");
            return null;
        }

        var prefabPath = "Assets/DMMAExporter/temp/TempAvatar.prefab";
        var model = bundle.LoadAsset<GameObject>(prefabPath);
        if (model == null)
        {
            Core.Error($"Failed to find prefab in bundle at path \"{prefabPath}\"");
            return null;
        }

        CleanPotentiallyDangerousGameObject(model);

        var instance = Object.Instantiate(model, Vector3.zero, Quaternion.identity);
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;

        return instance;
    }

    readonly static private HashSet<string> allowedComponentNames = [
        "MagicaCloth2.AutoRotate",
        "MagicaCloth2.CameraOrbit",
        "MagicaCloth2.ClothBehaviour",
        "MagicaCloth2.ColliderComponent",
        "MagicaCloth2.CreateSingleton",
        "MagicaCloth2.GameObjectContainer",
        "MagicaCloth2.MagicaCapsuleCollider",
        "MagicaCloth2.MagicaCloth",
        "MagicaCloth2.MagicaPlaneCollider",
        "MagicaCloth2.MagicaSettings",
        "MagicaCloth2.MagicaSphereCollider",
        "MagicaCloth2.MagicaWindZone",
        "MagicaCloth2.ModelController",
        "MagicaCloth2.RuntimeBuildDemo",
        "MagicaCloth2.RuntimeDressUpDemo",
        "MagicaCloth2.SimpleInputManager",
        "MagicaCloth2.SliderText",
        "MagicaCloth2.TargetFPS",
        "MagicaCloth2.WindDemo",
        "TMPro.TextContainer",
        "TMPro.TextMeshPro",
        "TMPro.TextMeshProUGUI",
        "TMPro.TMP_Dropdown",
        "TMPro.TMP_Dropdown+DropdownItem",
        "TMPro.TMP_InputField",
        "TMPro.TMP_ScrollbarEventHandler",
        "TMPro.TMP_SelectionCaret",
        "TMPro.TMP_SpriteAnimator",
        "TMPro.TMP_SubMesh",
        "TMPro.TMP_SubMeshUI",
        "TMPro.TMP_Text",
        "UnityEngine.AI.NavMeshAgent",
        "UnityEngine.AI.NavMeshObstacle",
        "UnityEngine.AI.OffMeshLink",
        "UnityEngine.AnchoredJoint2D",
        "UnityEngine.Animation",
        "UnityEngine.Animations.AimConstraint",
        "UnityEngine.Animations.LookAtConstraint",
        "UnityEngine.Animations.ParentConstraint",
        "UnityEngine.Animations.PositionConstraint",
        "UnityEngine.Animations.RotationConstraint",
        "UnityEngine.Animations.ScaleConstraint",
        "UnityEngine.Animator",
        "UnityEngine.AreaEffector2D",
        "UnityEngine.ArticulationBody",
        "UnityEngine.AudioBehaviour",
        "UnityEngine.AudioChorusFilter",
        "UnityEngine.AudioDistortionFilter",
        "UnityEngine.AudioEchoFilter",
        "UnityEngine.AudioHighPassFilter",
        "UnityEngine.AudioListener",
        "UnityEngine.AudioLowPassFilter",
        "UnityEngine.AudioReverbFilter",
        "UnityEngine.AudioReverbZone",
        "UnityEngine.AudioSource",
        "UnityEngine.Behaviour",
        "UnityEngine.BillboardRenderer",
        "UnityEngine.BoxCollider",
        "UnityEngine.BoxCollider2D",
        "UnityEngine.BuoyancyEffector2D",
        "UnityEngine.Camera",
        "UnityEngine.Canvas",
        "UnityEngine.CanvasGroup",
        "UnityEngine.CanvasRenderer",
        "UnityEngine.CapsuleCollider",
        "UnityEngine.CapsuleCollider2D",
        "UnityEngine.CharacterController",
        "UnityEngine.CharacterJoint",
        "UnityEngine.CircleCollider2D",
        "UnityEngine.Cloth",
        "UnityEngine.Collider",
        "UnityEngine.Collider2D",
        "UnityEngine.CompositeCollider2D",
        "UnityEngine.ConfigurableJoint",
        "UnityEngine.ConstantForce",
        "UnityEngine.ConstantForce2D",
        "UnityEngine.CustomCollider2D",
        "UnityEngine.DistanceJoint2D",
        "UnityEngine.EdgeCollider2D",
        "UnityEngine.Effector2D",
        "UnityEngine.EventSystems.BaseInput",
        "UnityEngine.EventSystems.BaseInputModule",
        "UnityEngine.EventSystems.BaseRaycaster",
        "UnityEngine.EventSystems.EventSystem",
        "UnityEngine.EventSystems.EventTrigger",
        "UnityEngine.EventSystems.Physics2DRaycaster",
        "UnityEngine.EventSystems.PhysicsRaycaster",
        "UnityEngine.EventSystems.PointerInputModule",
        "UnityEngine.EventSystems.StandaloneInputModule",
        "UnityEngine.EventSystems.TouchInputModule",
        "UnityEngine.EventSystems.UIBehaviour",
        "UnityEngine.FixedJoint",
        "UnityEngine.FixedJoint2D",
        "UnityEngine.FlareLayer",
        "UnityEngine.FrictionJoint2D",
        "UnityEngine.Grid",
        "UnityEngine.GridLayout",
        "UnityEngine.Halo",
        "UnityEngine.HingeJoint",
        "UnityEngine.HingeJoint2D",
        "UnityEngine.Joint",
        "UnityEngine.Joint2D",
        "UnityEngine.LensFlare",
        "UnityEngine.Light",
        "UnityEngine.LightAnchor",
        "UnityEngine.LightProbeGroup",
        "UnityEngine.LightProbeProxyVolume",
        "UnityEngine.LineRenderer",
        "UnityEngine.LODGroup",
        "UnityEngine.MeshCollider",
        "UnityEngine.MeshFilter",
        "UnityEngine.MeshRenderer",
        "UnityEngine.OcclusionArea",
        "UnityEngine.OcclusionPortal",
        "UnityEngine.ParticleSystem",
        "UnityEngine.ParticleSystemForceField",
        "UnityEngine.ParticleSystemRenderer",
        "UnityEngine.PhysicsUpdateBehaviour2D",
        "UnityEngine.PlatformEffector2D",
        "UnityEngine.Playables.PlayableDirector",
        "UnityEngine.PointEffector2D",
        "UnityEngine.PolygonCollider2D",
        "UnityEngine.Projector",
        "UnityEngine.RectTransform",
        "UnityEngine.ReflectionProbe",
        "UnityEngine.RelativeJoint2D",
        "UnityEngine.Renderer",
        "UnityEngine.Rendering.CameraSwitcher",
        "UnityEngine.Rendering.DebugUpdater",
        "UnityEngine.Rendering.FreeCamera",
        "UnityEngine.Rendering.LensFlareComponentSRP",
        "UnityEngine.Rendering.MousePositionDebug+GameViewEventCatcher",
        "UnityEngine.Rendering.ProbeTouchupVolume",
        "UnityEngine.Rendering.ProbeVolume",
        "UnityEngine.Rendering.ProbeVolumePerSceneData",
        "UnityEngine.Rendering.SceneRenderPipeline",
        "UnityEngine.Rendering.SortingGroup",
        "UnityEngine.Rendering.UI.DebugUIHandlerBitField",
        "UnityEngine.Rendering.UI.DebugUIHandlerButton",
        "UnityEngine.Rendering.UI.DebugUIHandlerCanvas",
        "UnityEngine.Rendering.UI.DebugUIHandlerColor",
        "UnityEngine.Rendering.UI.DebugUIHandlerContainer",
        "UnityEngine.Rendering.UI.DebugUIHandlerEnumField",
        "UnityEngine.Rendering.UI.DebugUIHandlerEnumHistory",
        "UnityEngine.Rendering.UI.DebugUIHandlerField`1",
        "UnityEngine.Rendering.UI.DebugUIHandlerFloatField",
        "UnityEngine.Rendering.UI.DebugUIHandlerFoldout",
        "UnityEngine.Rendering.UI.DebugUIHandlerGroup",
        "UnityEngine.Rendering.UI.DebugUIHandlerHBox",
        "UnityEngine.Rendering.UI.DebugUIHandlerIndirectFloatField",
        "UnityEngine.Rendering.UI.DebugUIHandlerIndirectToggle",
        "UnityEngine.Rendering.UI.DebugUIHandlerIntField",
        "UnityEngine.Rendering.UI.DebugUIHandlerMessageBox",
        "UnityEngine.Rendering.UI.DebugUIHandlerObject",
        "UnityEngine.Rendering.UI.DebugUIHandlerObjectList",
        "UnityEngine.Rendering.UI.DebugUIHandlerObjectPopupField",
        "UnityEngine.Rendering.UI.DebugUIHandlerPanel",
        "UnityEngine.Rendering.UI.DebugUIHandlerPersistentCanvas",
        "UnityEngine.Rendering.UI.DebugUIHandlerProgressBar",
        "UnityEngine.Rendering.UI.DebugUIHandlerRow",
        "UnityEngine.Rendering.UI.DebugUIHandlerToggle",
        "UnityEngine.Rendering.UI.DebugUIHandlerToggleHistory",
        "UnityEngine.Rendering.UI.DebugUIHandlerUIntField",
        "UnityEngine.Rendering.UI.DebugUIHandlerValue",
        "UnityEngine.Rendering.UI.DebugUIHandlerValueTuple",
        "UnityEngine.Rendering.UI.DebugUIHandlerVBox",
        "UnityEngine.Rendering.UI.DebugUIHandlerVector2",
        "UnityEngine.Rendering.UI.DebugUIHandlerVector3",
        "UnityEngine.Rendering.UI.DebugUIHandlerVector4",
        "UnityEngine.Rendering.UI.DebugUIHandlerWidget",
        "UnityEngine.Rendering.UI.UIFoldout",
        "UnityEngine.Rendering.Volume",
        "UnityEngine.Rigidbody",
        "UnityEngine.Rigidbody2D",
        "UnityEngine.SkinnedMeshRenderer",
        "UnityEngine.Skybox",
        "UnityEngine.SliderJoint2D",
        "UnityEngine.SphereCollider",
        "UnityEngine.SpringJoint",
        "UnityEngine.SpringJoint2D",
        "UnityEngine.SpriteMask",
        "UnityEngine.SpriteRenderer",
        "UnityEngine.StreamingController",
        "UnityEngine.SurfaceEffector2D",
        "UnityEngine.TargetJoint2D",
        "UnityEngine.Terrain",
        "UnityEngine.TerrainCollider",
        "UnityEngine.TextMesh",
        "UnityEngine.Tilemaps.Tilemap",
        "UnityEngine.Tilemaps.TilemapCollider2D",
        "UnityEngine.Tilemaps.TilemapRenderer",
        "UnityEngine.Timeline.SignalReceiver",
        "UnityEngine.TrailRenderer",
        "UnityEngine.Transform",
        "UnityEngine.Tree",
        "UnityEngine.U2D.Light2DBase",
        "UnityEngine.U2D.SpriteShapeRenderer",
        "UnityEngine.UI.AspectRatioFitter",
        "UnityEngine.UI.BaseMeshEffect",
        "UnityEngine.UI.Button",
        "UnityEngine.UI.CanvasScaler",
        "UnityEngine.UI.ContentSizeFitter",
        "UnityEngine.UI.Dropdown",
        "UnityEngine.UI.Dropdown+DropdownItem",
        "UnityEngine.UI.Graphic",
        "UnityEngine.UI.GraphicRaycaster",
        "UnityEngine.UI.GridLayoutGroup",
        "UnityEngine.UI.HorizontalLayoutGroup",
        "UnityEngine.UI.HorizontalOrVerticalLayoutGroup",
        "UnityEngine.UI.Image",
        "UnityEngine.UI.InputField",
        "UnityEngine.UI.LayoutElement",
        "UnityEngine.UI.LayoutGroup",
        "UnityEngine.UI.Mask",
        "UnityEngine.UI.MaskableGraphic",
        "UnityEngine.UI.Outline",
        "UnityEngine.UI.PositionAsUV1",
        "UnityEngine.UI.RawImage",
        "UnityEngine.UI.RectMask2D",
        "UnityEngine.UI.Scrollbar",
        "UnityEngine.UI.ScrollRect",
        "UnityEngine.UI.Selectable",
        "UnityEngine.UI.Shadow",
        "UnityEngine.UI.Slider",
        "UnityEngine.UI.Text",
        "UnityEngine.UI.Toggle",
        "UnityEngine.UI.ToggleGroup",
        "UnityEngine.UI.VerticalLayoutGroup",
        "UnityEngine.UIElements.PanelEventHandler",
        "UnityEngine.UIElements.PanelRaycaster",
        "UnityEngine.UIElements.UIDocument",
        "UnityEngine.Video.VideoPlayer",
        "UnityEngine.WheelCollider",
        "UnityEngine.WheelJoint2D",
        "UnityEngine.WindZone",
    ];
}
