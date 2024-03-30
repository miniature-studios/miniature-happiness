using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using System;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [Serializable]
    public class SettingsBinder : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] MainSettings mainSettings;
        [SerializeProperty(nameof(mainSettings))]
        public MainSettings MainSettings => mainSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] ComponentSettings componentSettings;
        [SerializeProperty(nameof(componentSettings))]
        public ComponentSettings ComponentSettings => componentSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] PuiSettings puiSettings;
        [SerializeProperty(nameof(puiSettings))]
        public PuiSettings PuiSettings => puiSettings.SetMonoBehaviour(monoBeh); 

        [SerializeField] MPUIKitSettings mpuikitSettings;
        [SerializeProperty(nameof(mpuikitSettings))]
        public MPUIKitSettings MPUIKitSettings => mpuikitSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] UnityImageSettings unityImageSettings;
        [SerializeProperty(nameof(unityImageSettings))]
        public UnityImageSettings UnityImageSettings => unityImageSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] Shapes2DSettings shapes2D_Settings;
        [SerializeProperty(nameof(shapes2D_Settings))]
        public Shapes2DSettings Shapes2DSettings => shapes2D_Settings.SetMonoBehaviour(monoBeh);

        [SerializeField] SpriteRendererSettings srSettings;
        [SerializeProperty(nameof(srSettings))]
        public SpriteRendererSettings SpriteRendererSettings => srSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] TextMeshSettings textMeshSettings;
        [SerializeProperty(nameof(textMeshSettings))]
        public TextMeshSettings TextMeshSettings => textMeshSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] UnityTextSettings unityTextSettings;
        [SerializeProperty(nameof(unityTextSettings))]
        public UnityTextSettings UnityTextSettings => unityTextSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] DebugSettings debugSettings;
        [SerializeProperty(nameof(debugSettings))]
        public DebugSettings DebugSettings => debugSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] PrefabSettings prefabSettings;
        [SerializeProperty(nameof(prefabSettings))]
        public PrefabSettings PrefabSettings => prefabSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] ButtonSettings buttonSettings;
        [SerializeProperty(nameof(buttonSettings))]
        public ButtonSettings ButtonSettings => buttonSettings.SetMonoBehaviour(monoBeh);

        [SerializeField] DAButtonSettings dabSettings;
        [SerializeProperty(nameof(dabSettings))]
        public DAButtonSettings DabSettings => dabSettings.SetMonoBehaviour(monoBeh);
    }
}