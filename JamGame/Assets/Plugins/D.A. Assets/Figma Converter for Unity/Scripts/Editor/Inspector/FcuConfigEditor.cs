using DA_Assets.Shared;
using UnityEditor;

namespace DA_Assets.FCU
{
    [CustomEditor(typeof(FcuConfig)), CanEditMultipleObjects]
    internal class FcuConfigEditor : Editor
    {
        private DAInspector gui;

        private void OnEnable()
        {
            if (gui == null)
                gui = DAInspector.Instance;
        }

        public override void OnInspectorGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = GuiStyle.DAInspectorBackground,
                DarkBg = true,
                Body = () =>
                {
                    base.OnInspectorGUI();
                }
            });
        }
    }
}