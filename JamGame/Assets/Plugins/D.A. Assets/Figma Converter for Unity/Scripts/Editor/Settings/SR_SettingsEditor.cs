using DA_Assets.Shared;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class SR_SettingsEditor : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_sr_settings.Localize());
            gui.Space15();

            monoBeh.Settings.SpriteRendererSettings.FlipX = gui.Toggle(new GUIContent(FcuLocKey.label_flip_x.Localize(), ""),
                monoBeh.Settings.SpriteRendererSettings.FlipX);

            monoBeh.Settings.SpriteRendererSettings.FlipY = gui.Toggle(new GUIContent(FcuLocKey.label_flip_y.Localize(), ""),
                monoBeh.Settings.SpriteRendererSettings.FlipY);

            monoBeh.Settings.SpriteRendererSettings.MaskInteraction = gui.EnumField(new GUIContent(FcuLocKey.label_mask_interaction.Localize(), ""),
                monoBeh.Settings.SpriteRendererSettings.MaskInteraction, uppercase: false);

            monoBeh.Settings.SpriteRendererSettings.SortPoint = gui.EnumField(new GUIContent(FcuLocKey.label_sort_point.Localize(), ""),
                monoBeh.Settings.SpriteRendererSettings.SortPoint);

            monoBeh.Settings.SpriteRendererSettings.SortingLayer = gui.TextField(new GUIContent(FcuLocKey.label_sorting_layer.Localize(), ""),
                monoBeh.Settings.SpriteRendererSettings.SortingLayer);

            monoBeh.Settings.SpriteRendererSettings.NextOrderStep = gui.IntField(new GUIContent(FcuLocKey.label_next_order_step.Localize(), ""),
                monoBeh.Settings.SpriteRendererSettings.NextOrderStep);
        }
    }
}
