using DA_Assets.Shared;

namespace DA_Assets.FCU
{
    internal class ButtonSettingsSection : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_button_settings.Localize());
            gui.Space15();

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.ButtonSettings.NormalColor);
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.ButtonSettings.HighlightedColor);
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.ButtonSettings.PressedColor);
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.ButtonSettings.SelectedColor);
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.ButtonSettings.DisabledColor);
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.ButtonSettings.FadeDuration);
        }
    }
}
