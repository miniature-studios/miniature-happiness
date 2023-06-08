using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
public class InspectorReadOnlyAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool prev_gui_state = GUI.enabled;
        GUI.enabled = false;
        _ = EditorGUI.PropertyField(position, property, label);
        GUI.enabled = prev_gui_state;
    }
}
#endif

public class InspectorReadOnlyAttribute : PropertyAttribute { }
