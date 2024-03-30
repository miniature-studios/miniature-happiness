using DA_Assets.Shared;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class PuiSettingsSection : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
#if PUI_EXISTS
            gui.SectionHeader(FcuLocKey.label_pui_settings.Localize());
            gui.Space15();

            monoBeh.Settings.PuiSettings.Type = gui.EnumField(new GUIContent(FcuLocKey.label_image_type.Localize(), ""),
                monoBeh.Settings.PuiSettings.Type);

            monoBeh.Settings.PuiSettings.RaycastTarget = gui.Toggle(new GUIContent(FcuLocKey.label_raycast_target.Localize(), ""),
                monoBeh.Settings.PuiSettings.RaycastTarget);

            monoBeh.Settings.PuiSettings.PreserveAspect = gui.Toggle(new GUIContent(FcuLocKey.label_preserve_aspect.Localize(), ""),
                monoBeh.Settings.PuiSettings.PreserveAspect);

            monoBeh.Settings.PuiSettings.RaycastPadding = gui.Vector4Field(new GUIContent(FcuLocKey.label_raycast_padding.Localize(), ""),
                monoBeh.Settings.PuiSettings.RaycastPadding);

            ///////////////////////////////////////////////

            monoBeh.Settings.PuiSettings.FalloffDistance = gui.FloatField(new GUIContent(FcuLocKey.label_pui_falloff_distance.Localize(), ""),
                monoBeh.Settings.PuiSettings.FalloffDistance);
#endif
        }
    }
}
