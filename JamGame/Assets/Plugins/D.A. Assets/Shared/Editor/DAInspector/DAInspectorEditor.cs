using UnityEditor;

namespace DA_Assets.Shared
{
    [CustomEditor(typeof(DAInspector)), CanEditMultipleObjects]
    internal class DAInspectorEditor : Editor
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