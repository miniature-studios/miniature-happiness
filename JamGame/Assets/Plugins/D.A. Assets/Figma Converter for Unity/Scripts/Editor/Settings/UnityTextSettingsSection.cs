using DA_Assets.Shared;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class UnityTextSettingsSection : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_unity_text_settings.Localize());
            gui.Space15();

            monoBeh.Settings.UnityTextSettings.BestFit = gui.Toggle(new GUIContent(FcuLocKey.label_best_fit.Localize(), FcuLocKey.tooltip_best_fit.Localize()),
                monoBeh.Settings.UnityTextSettings.BestFit);

            monoBeh.Settings.UnityTextSettings.FontLineSpacing = gui.FloatField(new GUIContent(FcuLocKey.label_line_spacing.Localize(), FcuLocKey.tooltip_line_spacing.Localize()),
                monoBeh.Settings.UnityTextSettings.FontLineSpacing);

            monoBeh.Settings.UnityTextSettings.HorizontalWrapMode = gui.EnumField(new GUIContent(FcuLocKey.label_horizontal_overflow.Localize(), FcuLocKey.tooltip_horizontal_overflow.Localize()),
                monoBeh.Settings.UnityTextSettings.HorizontalWrapMode);

            monoBeh.Settings.UnityTextSettings.VerticalWrapMode = gui.EnumField(new GUIContent(FcuLocKey.label_vertical_overflow.Localize(), FcuLocKey.tooltip_vertical_overflow.Localize()),
                monoBeh.Settings.UnityTextSettings.VerticalWrapMode);


            if (monoBeh.Settings.UnityTextSettings.VerticalWrapMode == VerticalWrapMode.Overflow)
            {
                monoBeh.Settings.UnityTextSettings.BestFit = false;
            }
        }
    }
}
