#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class InspectorReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
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