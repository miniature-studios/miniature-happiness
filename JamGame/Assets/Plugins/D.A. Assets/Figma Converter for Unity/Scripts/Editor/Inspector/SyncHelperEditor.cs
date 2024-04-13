using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using UnityEditor;

namespace DA_Assets.FCU
{
    [CustomEditor(typeof(SyncHelper)), CanEditMultipleObjects]
    internal class SyncHelperEditor : Editor
    {
        private DAInspector gui;
        private FigmaConverterUnity fcu;
        SyncHelper syncObject;
        private void OnEnable()
        {
             syncObject = (SyncHelper)target;
            fcu = syncObject.Data.FigmaConverterUnity;
            if (gui == null)
                gui = DAInspector.Instance;
        }

        public override void OnInspectorGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = GuiStyle.Background,
                Body = () =>
                {
                    gui.Label12px(FcuLocKey.label_dont_remove_fcu_meta.Localize(), widthType: WidthType.Expand);
                    gui.Label10px(FcuLocKey.label_more_about_layout_updating.Localize(), widthType: WidthType.Expand);

                    if ((fcu?.Settings.DebugSettings.DebugMode).ToBoolNullFalse())
                    {
                        gui.Space15();
                        EditorGUILayout.Vector3Field("World Position", syncObject.transform.position);
                        EditorGUILayout.Vector3Field("Local Position", syncObject.transform.localPosition);
                        gui.Space15();
                        base.OnInspectorGUI();
                    }
                }
            });
        }
    }
}