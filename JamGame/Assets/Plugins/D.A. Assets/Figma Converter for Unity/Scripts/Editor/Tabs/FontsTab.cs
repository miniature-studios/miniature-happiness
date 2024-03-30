using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class FontsTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_font_settings.Localize());
            gui.Space15();

            DrawPathSettings();

            gui.Space15();

            DrawGoogleFontsSettings();

#if TextMeshPro
            gui.Space15();
            DrawFontGenerationSettings();
#endif

            gui.Space30();
        }

        private void DrawFontGenerationSettings()
        {
            gui.SectionHeader(FcuLocKey.label_asset_creator_settings.Localize(), FcuLocKey.tooltip_asset_creator_settings.Localize());
            gui.Space15();

            monoBeh.FontDownloader.TmpDownloader.SamplingPointSize = gui.IntField(new GUIContent(FcuLocKey.label_sampling_point_size.Localize(), FcuLocKey.tooltip_sampling_point_size.Localize()),
                monoBeh.FontDownloader.TmpDownloader.SamplingPointSize);

            monoBeh.FontDownloader.TmpDownloader.AtlasPadding = gui.IntField(new GUIContent(FcuLocKey.label_atlas_padding.Localize(), FcuLocKey.tooltip_atlas_padding.Localize()),
                monoBeh.FontDownloader.TmpDownloader.AtlasPadding);

            monoBeh.FontDownloader.TmpDownloader.RenderMode = gui.EnumField(new GUIContent(FcuLocKey.label_render_mode.Localize(), FcuLocKey.tooltip_render_mode.Localize()),
                monoBeh.FontDownloader.TmpDownloader.RenderMode);

            Vector2Int atlasResolution = new Vector2Int(monoBeh.FontDownloader.TmpDownloader.AtlasWidth, monoBeh.FontDownloader.TmpDownloader.AtlasHeight);
            atlasResolution = gui.Vector2IntField(new GUIContent(FcuLocKey.label_atlas_resolution.Localize(), FcuLocKey.tooltip_atlas_resolution.Localize()), atlasResolution);
            monoBeh.FontDownloader.TmpDownloader.AtlasWidth = atlasResolution.x;
            monoBeh.FontDownloader.TmpDownloader.AtlasHeight = atlasResolution.y;

#if TextMeshPro
            monoBeh.FontDownloader.TmpDownloader.AtlasPopulationMode = gui.EnumField(new GUIContent(FcuLocKey.label_atlas_population_mode.Localize(), FcuLocKey.tooltip_atlas_population_mode.Localize()),
                monoBeh.FontDownloader.TmpDownloader.AtlasPopulationMode);
#endif
            monoBeh.FontDownloader.TmpDownloader.EnableMultiAtlasSupport = gui.Toggle(new GUIContent(FcuLocKey.label_enable_multi_atlas_support.Localize(), FcuLocKey.tooltip_enable_multi_atlas_support.Localize()),
               monoBeh.FontDownloader.TmpDownloader.EnableMultiAtlasSupport);

            gui.Space15();

            if (gui.OutlineButton(FcuLocKey.label_download_fonts_from_project.Localize(monoBeh.Settings.ComponentSettings.TextComponent)))
            {
                monoBeh.FontDownloader.DownloadAllProjectFonts().StartDARoutine(monoBeh);
            }
        }

        private void DrawGoogleFontsSettings()
        {
            gui.SectionHeader(FcuLocKey.label_google_fonts_settings.Localize());
            gui.Space15();

            FcuConfig.Instance.GoogleFontsApiKey = gui.TextField(
                new GUIContent(FcuLocKey.label_google_fonts_api_key.Localize(), FcuLocKey.tooltip_google_fonts_api_key.Localize(FcuLocKey.label_google_fonts_api_key.Localize())),
                FcuConfig.Instance.GoogleFontsApiKey,
                new GUIContent(FcuLocKey.label_get_google_api_key.Localize()), () =>
                {
                    Application.OpenURL("https://developers.google.com/fonts/docs/developer_api#identifying_your_application_to_google");
                });

            gui.Space5();

            monoBeh.FontDownloader.GFontsApi.FontSubsets |= FontSubset.Latin;

            gui.SerializedPropertyField<FigmaConverterUnity>(
                scriptableObject.SerializedObject, x => x.FontDownloader.GFontsApi.FontSubsets);
        }

        private void DrawPathSettings()
        {
            monoBeh.FontLoader.TtfFontsPath = gui.DrawSelectPathField(
                monoBeh.FontLoader.TtfFontsPath,
                new GUIContent(FcuLocKey.label_ttf_path.Localize(), ""),
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_fonts_folder.Localize());

            gui.Space5();

            if (gui.OutlineButton(FcuLocKey.label_add_ttf_fonts_from_folder.Localize(), FcuLocKey.tooltip_add_fonts_from_folder.Localize()))
            {
                monoBeh.FontLoader.AddToTtfFontsList().StartDARoutine(monoBeh);
            }

            gui.Space5();

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.FontLoader.TtfFonts);

#if TextMeshPro
            gui.Space15();
            monoBeh.FontLoader.TmpFontsPath = gui.DrawSelectPathField(
                monoBeh.FontLoader.TmpFontsPath,
                new GUIContent(FcuLocKey.label_tmp_path.Localize(), ""),
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_fonts_folder.Localize());

            gui.Space5();

            if (gui.OutlineButton(FcuLocKey.label_add_tmp_fonts_from_folder.Localize(), FcuLocKey.tooltip_add_fonts_from_folder.Localize()))
            {
                monoBeh.FontLoader.AddToTmpMeshFontsList().StartDARoutine(monoBeh);
            }

            gui.Space5();

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.FontLoader.TmpFonts);
#endif
        }
    }
}