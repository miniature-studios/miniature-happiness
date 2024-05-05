using DA_Assets.Shared;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class PrefabSettingsTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_prefab_settings.Localize());
            gui.Space15();

            monoBeh.PrefabCreator.PrefabsPath = gui.DrawSelectPathField(
                monoBeh.PrefabCreator.PrefabsPath,
                new GUIContent(FcuLocKey.label_prefabs_path.Localize(), FcuLocKey.tooltip_prefabs_path.Localize()),
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_prefabs_folder.Localize());

            monoBeh.Settings.PrefabSettings.TextPrefabNameType = gui.EnumField(
                new GUIContent(FcuLocKey.label_text_prefab_naming_mode.Localize(), ""),
                monoBeh.Settings.PrefabSettings.TextPrefabNameType,
                false,
                new string[]
                {
                    FcuLocKey.label_humanized_color.Localize(),
                    FcuLocKey.label_hex_color.Localize(),
                    FcuLocKey.label_figma_color.Localize()
                });
        }
    }
}