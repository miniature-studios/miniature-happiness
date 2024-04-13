using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using DA_Assets.FCU.Extensions;

namespace DA_Assets.FCU
{
    internal class MainSettingsTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_main_settings.Localize());
            gui.Space15();

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    monoBeh.FigmaSession.Token = gui.BigTextField(
                        monoBeh.FigmaSession.Token,
                        FcuLocKey.label_token.Localize(),
                        FcuLocKey.tooltip_token.Localize(),
                        password: true);

                    gui.Space5();

                    var gr = new Group();
                    gr.Style = GuiStyle.Group2Buttons;

                    gr.GroupType = GroupType.Horizontal;
                    gr.Body = () =>
                    {
                        if (gui.SquareButton30x30(new GUIContent(gui.Resources.ImgViewRecent, FcuLocKey.tooltip_recent_tokens.Localize())))
                        {
                            ShowRecentSessionsPopup_OnClick();
                        }

                        gui.Space5();
                        if (gui.SquareButton30x30(new GUIContent(gui.Resources.IconAuth, FcuLocKey.tooltip_auth.Localize())))
                        {
                            monoBeh.EventHandlers.Auth_OnClick();
                        }
                    };
                    gui.DrawGroup(gr);

                }
            });

            gui.Space6();

#if UITKPLUGIN_EXISTS
            monoBeh.Settings.MainSettings.UIFramework = gui.EnumField(
                new GUIContent(FcuLocKey.label_ui_framework.Localize(), FcuLocKey.tooltip_ui_framework.Localize()),
                monoBeh.Settings.MainSettings.UIFramework);
#endif
            monoBeh.Settings.MainSettings.ImageFormat = gui.EnumField(
                new GUIContent(FcuLocKey.label_images_format.Localize(), FcuLocKey.tooltip_images_format.Localize()),
                monoBeh.Settings.MainSettings.ImageFormat);

            monoBeh.Settings.MainSettings.ImageScale = gui.SliderField(
                 new GUIContent(FcuLocKey.label_images_scale.Localize(), FcuLocKey.tooltip_images_scale.Localize()),
                monoBeh.Settings.MainSettings.ImageScale, 0.25f, 4.0f).RoundToNearest025();

            monoBeh.Settings.MainSettings.PixelsPerUnit = gui.FloatField(
                 new GUIContent(FcuLocKey.label_pixels_per_unit.Localize(), FcuLocKey.tooltip_pixels_per_unit.Localize()),
                monoBeh.Settings.MainSettings.PixelsPerUnit);

            monoBeh.Settings.MainSettings.GameObjectLayer = gui.LayerField(
                new GUIContent(FcuLocKey.label_go_layer.Localize(), FcuLocKey.tooltip_go_layer.Localize()),
                monoBeh.Settings.MainSettings.GameObjectLayer);

            monoBeh.Settings.MainSettings.PositioningMode = gui.EnumField(
                new GUIContent(FcuLocKey.label_positioning_mode.Localize(), FcuLocKey.tooltip_positioning_mode.Localize()),
                monoBeh.Settings.MainSettings.PositioningMode);

            monoBeh.Settings.MainSettings.PivotType = gui.EnumField(
                new GUIContent(FcuLocKey.label_pivot_type.Localize(), FcuLocKey.tooltip_pivot_type.Localize()),
                monoBeh.Settings.MainSettings.PivotType, uppercase: false);

            monoBeh.Settings.MainSettings.SpritesPath = gui.DrawSelectPathField(
                monoBeh.Settings.MainSettings.SpritesPath,
                new GUIContent(FcuLocKey.label_sprites_path.Localize(), FcuLocKey.tooltip_sprites_path.Localize()),
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_folder.Localize());

#if UITKPLUGIN_EXISTS
            monoBeh.Settings.MainSettings.UGUIOutputPath = gui.DrawSelectPathField(
              monoBeh.Settings.MainSettings.UGUIOutputPath,
              new GUIContent(FcuLocKey.label_uitk_output_path.Localize(), FcuLocKey.tooltip_uitk_output_path.Localize()),
              new GUIContent(FcuLocKey.label_change.Localize()),
              FcuLocKey.label_select_folder.Localize());
#endif
            monoBeh.Settings.MainSettings.RedownloadSprites = gui.Toggle(
                new GUIContent(FcuLocKey.label_redownload_sprites.Localize(), FcuLocKey.tooltip_redownload_sprites.Localize()),
                monoBeh.Settings.MainSettings.RedownloadSprites);

            monoBeh.Settings.MainSettings.RawImport = gui.Toggle(
                new GUIContent(FcuLocKey.label_raw_import.Localize(), FcuLocKey.tooltip_raw_import.Localize()),
                monoBeh.Settings.MainSettings.RawImport);
        }

        private void ShowRecentSessionsPopup_OnClick()
        {
            if (monoBeh.IsJsonNetExists() == false)
            {
                DALogger.LogError(FcuLocKey.log_cant_find_package.Localize(DAConstants.JsonNetPackageName));
                return;
            }

            FigmaSessionItem[] recentSessions = monoBeh.FigmaSession.GetItems();

            List<GUIContent> options = new List<GUIContent>();

            if (recentSessions.IsEmpty())
            {
                options.Add(new GUIContent(FcuLocKey.label_no_recent_sessions.Localize()));
            }
            else
            {
                foreach (FigmaSessionItem session in recentSessions)
                {
                    options.Add(new GUIContent($"{session.Name} | {session.Email}"));
                }
            }

            options.Add(new GUIContent($"Add new"));

            EditorUtility.DisplayCustomMenu(new Rect(11, 90, 0, 0), options.ToArray(), -1, (userData, ops, selected) =>
            {
                if (selected == options.Count - 1)
                {
                    monoBeh.FigmaSession.AddNew(monoBeh.FigmaSession.Token);
                }
                else
                {
                    monoBeh.FigmaSession.AddNew(recentSessions[selected].Token);
                }
            }, null);
        }
    }
}