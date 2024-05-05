using DA_Assets.Shared;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class MPImageSettingsSection : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
#if MPUIKIT_EXISTS
            gui.SectionHeader(FcuLocKey.label_mpuikit_settings.Localize());
            gui.Space15();

            monoBeh.Settings.MPUIKitSettings.Type = gui.EnumField(new GUIContent(FcuLocKey.label_image_type.Localize(), ""),
                monoBeh.Settings.MPUIKitSettings.Type);

            monoBeh.Settings.MPUIKitSettings.RaycastTarget = gui.Toggle(new GUIContent(FcuLocKey.label_raycast_target.Localize(), ""),
                monoBeh.Settings.MPUIKitSettings.RaycastTarget);

            monoBeh.Settings.MPUIKitSettings.PreserveAspect = gui.Toggle(new GUIContent(FcuLocKey.label_preserve_aspect.Localize(), ""),
                monoBeh.Settings.MPUIKitSettings.PreserveAspect);

            monoBeh.Settings.MPUIKitSettings.RaycastPadding = gui.Vector4Field(new GUIContent(FcuLocKey.label_raycast_padding.Localize(), ""),
                monoBeh.Settings.MPUIKitSettings.RaycastPadding);

            ///////////////////////////////////////////////

            monoBeh.Settings.MPUIKitSettings.FalloffDistance = gui.FloatField(new GUIContent(FcuLocKey.label_pui_falloff_distance.Localize(), ""),
                monoBeh.Settings.MPUIKitSettings.FalloffDistance);
#endif
        }
    }
}
