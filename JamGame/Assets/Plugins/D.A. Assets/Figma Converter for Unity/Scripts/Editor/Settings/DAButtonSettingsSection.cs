using DA_Assets.FCU.Extensions;
using DA_Assets.Shared;
using UnityEngine;
using DA_Assets.Shared.Extensions;

#if DABUTTON_EXISTS
using DA_Assets.DAB;
#endif

namespace DA_Assets.FCU
{
    internal class DAButtonSettingsSection : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_dabutton_settings.Localize());
            gui.Space15();

            monoBeh.Settings.DabSettings.BlendMode = gui.EnumField(new GUIContent("Blend Mode"), monoBeh.Settings.DabSettings.BlendMode);
            monoBeh.Settings.DabSettings.BlendIntensity = gui.FloatField(new GUIContent("Blend Intensity"), monoBeh.Settings.DabSettings.BlendIntensity);

#if DABUTTON_EXISTS
            if (monoBeh.Settings.DabSettings.DefaultTargetGraphic.IsDefault())
            {
                monoBeh.Settings.DabSettings.DefaultTargetGraphic = DAButtonDefaults.Instance.CopyTargetGraphic(DAButtonDefaults.Instance.DefaultTargetGraphic);
            }

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.DabSettings.DefaultTargetGraphic, isExpanded: true);
#endif
        }
    }
}
