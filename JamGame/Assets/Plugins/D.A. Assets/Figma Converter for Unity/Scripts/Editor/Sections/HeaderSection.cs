using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class HeaderSection : ScriptableObjectBinder<FcuEditor, FigmaConverterUnity>
    {
        private void DrawStar()
        {
            GUILayout.Box(gui.Resources.ImgStar, gui.GetStyle(GuiStyle.ImgStar));
        }
        
        private void RateMeUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    gui.FlexibleSpace();

                    for (int i = 0; i < 5; i++)
                    {
                        DrawStar();

                        if (i != 5)
                        {
                            gui.Space5();
                        }
                    }

                    gui.FlexibleSpace();
                }
            });

            gui.Space15();

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = GuiStyle.BoxPanel,
                Body = () =>
                {
                    int dc = GetFirstVersionDaysCount();
                    gui.Label(new GUIContent(FcuLocKey.label_rateme_desc.Localize(dc)), WidthType.Expand, GuiStyle.Label12px);

                    gui.Space5();
                    if (gui.OutlineButton("Don't show", null, expand: WidthType.Expand))
                    {
                        DontShowRateMe_OnClick();
                    }

                    gui.Space5();
                    if (gui.OutlineButton("Open Asset Store", null, expand: WidthType.Expand))
                    {
                        Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/198134#reviews");
                        DontShowRateMe_OnClick();
                    }
                }
            });
        }

        private void DontShowRateMe_OnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt(FcuConfig.RATEME_PREFS_KEY, 1);
#endif
        }

        public void Draw()
        {
            gui.TopProgressBar(monoBeh.RequestSender.PbarProgress);

            GUILayout.BeginVertical(gui.Resources.FcuLogo, gui.GetStyle(GuiStyle.Logo));
            gui.Space30();
            GUILayout.EndVertical();

            if (monoBeh.AssetTools.NeedShowRateMe)
            {
                RateMeUI();
                gui.Space(25);
            }

            UpdateChecker.DrawVersionLine(AssetType.fcu, FcuConfig.Instance.ProductVersion);
#if UITKPLUGIN_EXISTS
            UpdateChecker.DrawVersionLine(AssetType.uitk, FuitkConfig.Instance.ProductVersion);
#endif
            DrawImportInfoLine();
            DrawCurrentProjectName();
        }

        private static int GetFirstVersionDaysCount()
        {
            try
            {
                Asset assetInfo = DAWebConfig.WebConfig.Assets.FirstOrDefault(x => x.Type == AssetType.fcu);
                AssetVersion firstVersion = assetInfo.Versions.First();

                DateTime firstDt = DateTime.ParseExact(firstVersion.ReleaseDate, "MMM d, yyyy", new System.Globalization.CultureInfo("en-US"));

                int dc = (int)Mathf.Abs((float)(DateTime.Now - firstDt).TotalDays);

                return dc;
            }
            catch
            {
                return -1;
            }
        }
        public void DrawSmallHeader()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Body = () =>
                {
                    DrawImportInfoLine();
                    DrawCurrentProjectName();
                }
            });
        }

        private void DrawImportInfoLine()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    gui.FlexibleSpace();

                    gui.Label(new GUIContent($"{Mathf.Round(monoBeh.RequestSender.PbarBytes / 1024)} kB", FcuLocKey.label_kilobytes.Localize()), WidthType.Option, GuiStyle.Label10px);
                    gui.Space5();
                    gui.Label(new GUIContent("—"), WidthType.Option, GuiStyle.Label10px);

                    string userId = monoBeh.FigmaSession.CurrentFigmaUser.Id.SubstringSafe(10);
                    string userName = monoBeh.FigmaSession.CurrentFigmaUser.Name;

                    if (string.IsNullOrWhiteSpace(userName) == false)
                    {
                        gui.Space5();
                        gui.Label(new GUIContent(userName, FcuLocKey.label_user_name.Localize()), WidthType.Option, GuiStyle.Label10px);
                        gui.Space5();
                        gui.Label(new GUIContent("—"), WidthType.Option, GuiStyle.Label10px);
                    }
                    else if (string.IsNullOrWhiteSpace(userId) == false)
                    {
                        gui.Space5();
                        gui.Label(new GUIContent(userId, FcuLocKey.tooltip_user_id.Localize()), WidthType.Option, GuiStyle.Label10px);
                        gui.Space5();
                        gui.Label(new GUIContent("—"), WidthType.Option, GuiStyle.Label10px);
                    }

                    gui.Space5();
                    gui.Label(new GUIContent(monoBeh.Guid, FcuLocKey.tooltip_asset_instance_id.Localize()), WidthType.Option, GuiStyle.Label10px);
                }
            });
        }

        private void DrawCurrentProjectName()
        {
            string currentProjectName = monoBeh.CurrentProject.FigmaProject.Name;

            if (currentProjectName != null)
            {
                gui.DrawGroup(new Group
                {
                    GroupType = GroupType.Horizontal,
                    Body = () =>
                    {
                        gui.FlexibleSpace();
                        gui.Label10px(currentProjectName, widthType: WidthType.Option);
                    }
                });
            }
        }
    }
}