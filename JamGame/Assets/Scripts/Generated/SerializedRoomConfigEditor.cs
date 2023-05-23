#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializedRoomConfig))]
public class SerializedRoomConfigDrawer : PropertyDrawer
{
    private readonly string[] implementingTypeNames = { "FixedRoomConfig", "RandomRoomConfig" };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty selected_type_prop = property.FindPropertyRelative("selectedType");
        string selected_type = selected_type_prop.stringValue;
        if (selected_type == "")
        {
            selected_type_prop.stringValue = implementingTypeNames[0];
            selected_type = selected_type_prop.stringValue;
        }

        Rect pos = position;
        pos.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginChangeCheck();
        int selected_index = 0;
        for (int i = 0; i < implementingTypeNames.Length; i++)
        {
            if (implementingTypeNames[i] == selected_type)
            {
                selected_index = i;
                break;
            }
        }

        int new_selected_index = EditorGUI.Popup(pos, selected_index, implementingTypeNames);
        if (EditorGUI.EndChangeCheck())
        {
            selected_type_prop.stringValue = implementingTypeNames[new_selected_index];
            selected_type = selected_type_prop.stringValue;
        }

        pos.y += EditorGUIUtility.singleLineHeight;
        pos.height = position.height - EditorGUIUtility.singleLineHeight;

        SerializedProperty typed_prop =
            property.FindPropertyRelative(PascalToCamelCase(selected_type));
        _ = EditorGUI.PropertyField(pos, typed_prop, true);
    }

    private string PascalToCamelCase(string pascal)
    {
        return pascal[..1].ToLower() + pascal[1..];
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty selected_type_prop = property.FindPropertyRelative("selectedType");
        string selected_type = selected_type_prop.stringValue;
        float height = 0.0f;
        if (selected_type != "")
        {
            var typed_prop = property.FindPropertyRelative(PascalToCamelCase(selected_type));
            height = EditorGUI.GetPropertyHeight(typed_prop);
        }
        return EditorGUIUtility.singleLineHeight + height;
    }
}
#endif