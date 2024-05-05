using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class ComparerWindow : EditorWindow
    {
        private static Vector2 windowSize = new Vector2(410, 290);
        private SyncHelper sh1, sh2;
        private DAInspector gui => DAInspector.Instance;

        public static void Show(SyncHelper sh1, SyncHelper sh2)
        {
            ComparerWindow win = GetWindow<ComparerWindow>(FcuLocKey.label_object_comparer.Localize());
            win.SetModel(sh1, sh2);

            win.position = new Rect(
                (Screen.currentResolution.width - windowSize.x * 2) / 2,
                (Screen.currentResolution.height - windowSize.y * 2) / 2,
                windowSize.x,
                windowSize.y);
        }

        public void SetModel(SyncHelper sh1, SyncHelper sh2)
        {
            this.sh1 = sh1;
            this.sh2 = sh2;
        }

        private void OnGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = GuiStyle.TabBg2,
                Scroll = true,
                Body = () =>
                {
                    gui.Label12px(FcuLocKey.label_comparer_desc.Localize(FcuLocKey.label_open_diff_checker.Localize()));

                    gui.Space5();

                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Horizontal,
                        Body = () =>
                        {
                            gui.DrawGroup(new Group
                            {
                                GroupType = GroupType.Vertical,
                                Body = () =>
                                {
                                    GUILayout.TextArea(sh1.Data.HashDataTree, GUILayout.Height(100));

                                    gui.Space5();

                                    if (gui.OutlineButton(FcuLocKey.label_copy_to_clipboard.Localize(), expand: WidthType.Expand))
                                    {
                                        GUIUtility.systemCopyBuffer = sh1.Data.HashDataTree;
                                    }
                                }
                            });

                            gui.Space5();

                            gui.DrawGroup(new Group
                            {
                                GroupType = GroupType.Vertical,
                                Body = () =>
                                {
                                    GUILayout.TextArea(sh2.Data.HashDataTree, GUILayout.Height(100));

                                    gui.Space5();

                                    if (gui.OutlineButton(FcuLocKey.label_copy_to_clipboard.Localize(), expand: WidthType.Expand))
                                    {
                                        GUIUtility.systemCopyBuffer = sh2.Data.HashDataTree;
                                    }
                                }
                            });
                        }
                    });

                    gui.Space5();

                    if (gui.OutlineButton(FcuLocKey.label_open_diff_checker.Localize(), expand: WidthType.Expand))
                    {
                        Application.OpenURL($"https://www.diffchecker.com/");
                    }
                }
            });
        }
    }
}