using DA_Assets.Shared;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class Shapes2DSettingsSection : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
#if SHAPES_EXISTS
            gui.SectionHeader(FcuLocKey.label_shapes2d_settings.Localize());
            gui.Space15();

            monoBeh.Settings.Shapes2DSettings.Type = gui.EnumField(new GUIContent(FcuLocKey.label_image_type.Localize(), ""),
                monoBeh.Settings.Shapes2DSettings.Type);

            monoBeh.Settings.Shapes2DSettings.RaycastTarget = gui.Toggle(new GUIContent(FcuLocKey.label_raycast_target.Localize(), ""),
                monoBeh.Settings.Shapes2DSettings.RaycastTarget);

            monoBeh.Settings.Shapes2DSettings.PreserveAspect = gui.Toggle(new GUIContent(FcuLocKey.label_preserve_aspect.Localize(), ""),
                monoBeh.Settings.Shapes2DSettings.PreserveAspect);

            monoBeh.Settings.Shapes2DSettings.RaycastPadding = gui.Vector4Field(new GUIContent(FcuLocKey.label_raycast_padding.Localize(), ""),
                monoBeh.Settings.Shapes2DSettings.RaycastPadding);
#endif
        }
    }
}
