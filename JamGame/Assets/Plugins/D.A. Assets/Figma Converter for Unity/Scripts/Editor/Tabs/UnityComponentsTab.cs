using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using UnityEngine;

#pragma warning disable IDE0003
#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    internal class UnityComponentsTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_import_components.Localize(), FcuLocKey.tooltip_import_components.Localize());
            gui.Space15();

            monoBeh.Settings.ComponentSettings.ImageComponent = gui.EnumField(
                new GUIContent(FcuLocKey.label_image_component.Localize(), FcuLocKey.tooltip_image_component.Localize()),
                monoBeh.Settings.ComponentSettings.ImageComponent, true);

            monoBeh.Settings.ComponentSettings.TextComponent = gui.EnumField(
                new GUIContent(FcuLocKey.label_text_component.Localize(), FcuLocKey.tooltip_text_component.Localize()),
                monoBeh.Settings.ComponentSettings.TextComponent, true);

            monoBeh.Settings.ComponentSettings.ShadowComponent = gui.EnumField(
                new GUIContent(FcuLocKey.label_shadow_type.Localize(), FcuLocKey.tooltip_shadow_type.Localize()),
                monoBeh.Settings.ComponentSettings.ShadowComponent, true);

            monoBeh.Settings.ComponentSettings.ButtonComponent = gui.EnumField(
                new GUIContent(FcuLocKey.label_button_type.Localize(), FcuLocKey.tooltip_button_type.Localize()),
                monoBeh.Settings.ComponentSettings.ButtonComponent, true, null);

            monoBeh.Settings.ComponentSettings.UseI2Localization = gui.Toggle(
                new GUIContent(FcuLocKey.label_use_i2localization.Localize(), FcuLocKey.tooltip_use_i2localization.Localize()),
                monoBeh.Settings.ComponentSettings.UseI2Localization);

            gui.Space15();

            switch (monoBeh.Settings.ComponentSettings.ImageComponent)
            {
                case ImageComponent.UnityImage:
                case ImageComponent.RawImage:
                    this.UnityImageSettingsTab.Draw();
                    break;
                case ImageComponent.Shape:
                    this.Shapes2DSettingsTab.Draw();
                    break;
                case ImageComponent.ProceduralImage:
                    this.PuiSettingsTab.Draw();
                    break;
                case ImageComponent.MPImage:
                    this.MPImageSettingsTab.Draw();
                    break;
                case ImageComponent.SpriteRenderer:
                    this.SR_SettingsEditor.Draw();
                    break;
            }

            gui.Space15();

            switch (monoBeh.Settings.ComponentSettings.TextComponent)
            {
                case TextComponent.UnityText:
                    this.DefaultTextSettingsTab.Draw();
                    break;
                case TextComponent.TextMeshPro:
                    this.TextMeshProSettingsTab.Draw();
                    break;
            }

            gui.Space15();
            this.ButtonSettingsTab.Draw();
#if DABUTTON_EXISTS
            gui.Space15();
            this.DabSettingsTab.Draw();
#endif

            gui.Space30();
        }

        private UnityImageSettingsSection unityImageSettingsTab;
        internal UnityImageSettingsSection UnityImageSettingsTab => monoBeh.Bind(ref unityImageSettingsTab, scriptableObject);

        private Shapes2DSettingsSection shapesSettingsTab;
        internal Shapes2DSettingsSection Shapes2DSettingsTab => monoBeh.Bind(ref shapesSettingsTab, scriptableObject);

        private PuiSettingsSection puiSettingsTab;
        internal PuiSettingsSection PuiSettingsTab => monoBeh.Bind(ref puiSettingsTab, scriptableObject);

        private MPImageSettingsSection mpImageSettingsTab;
        internal MPImageSettingsSection MPImageSettingsTab => monoBeh.Bind(ref mpImageSettingsTab, scriptableObject);


        private SR_SettingsEditor srSettings;
        public SR_SettingsEditor SR_SettingsEditor => monoBeh.Bind(ref srSettings, scriptableObject);

        private TextMeshProSettingsSection textMeshProSettingsTab;
        internal TextMeshProSettingsSection TextMeshProSettingsTab => monoBeh.Bind(ref textMeshProSettingsTab, scriptableObject);

        private UnityTextSettingsSection defaultTextSettingsTab;
        internal UnityTextSettingsSection DefaultTextSettingsTab => monoBeh.Bind(ref defaultTextSettingsTab, scriptableObject);


        private ButtonSettingsSection buttonSettingsTab;
        internal ButtonSettingsSection ButtonSettingsTab => monoBeh.Bind(ref buttonSettingsTab, scriptableObject);

        private DAButtonSettingsSection dabSettingsTab;
        internal DAButtonSettingsSection DabSettingsTab => monoBeh.Bind(ref dabSettingsTab, scriptableObject);
    }
}