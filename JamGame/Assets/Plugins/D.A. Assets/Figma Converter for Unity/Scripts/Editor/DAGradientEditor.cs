using DA_Assets.Shared;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.DAG
{
    [CustomEditor(typeof(DAGradient)), CanEditMultipleObjects]
    public class DAGradientEditor : DaiEditor<DAGradientEditor, DAGradient>
    {
        public override void OnInspectorGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = GuiStyle.Background,
                LabelWidth = 80,
                Body = () =>
                {
                    DrawAssetLogo();

                    gui.SerializedPropertyField<DAGradient>(serializedObject, x => x.Gradient, false);
                    gui.Space5();
                    gui.SerializedPropertyField<DAGradient>(serializedObject, x => x.BlendMode);
                    gui.Space5();
                    gui.SerializedPropertyField<DAGradient>(serializedObject, x => x.Intensity);
                    gui.Space5();
                    gui.SerializedPropertyField<DAGradient>(serializedObject, x => x.Angle);

                    Footer.DrawFooter();
                }
            });
        }

        private void DrawAssetLogo()
        {
            GUILayout.BeginVertical(gui.Resources.DAGradientLogo, gui.GetStyle(GuiStyle.Logo));
            gui.Space60();
            GUILayout.EndVertical();
        }
    }
}