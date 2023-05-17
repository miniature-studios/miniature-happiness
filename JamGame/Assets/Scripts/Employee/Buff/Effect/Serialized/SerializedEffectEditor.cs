#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializedEffect))]
public class SerializedEffectDrawer : PropertyDrawer
{
    private string[] effectTypeNames;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (effectTypeNames == null)
        {
            InitEffectTypeNames();
        }

        SerializedProperty effect_type_prop = property.FindPropertyRelative("effectType");
        if (effect_type_prop.stringValue == "")
        {
            effect_type_prop.stringValue = effectTypeNames[0];
        }

        SerializedProperty effect_prop = property.FindPropertyRelative(effect_type_prop.stringValue);

        Rect pos = position;
        pos.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginChangeCheck();
        int selected_index = 0;
        for (int i = 0; i < effectTypeNames.Length; i++)
        {
            if (effectTypeNames[i] == effect_type_prop.stringValue)
            {
                selected_index = i;
                break;
            }
        }

        int new_selected_index = EditorGUI.Popup(pos, selected_index, effectTypeNames);
        if (EditorGUI.EndChangeCheck())
        {
            effect_type_prop.stringValue = effectTypeNames[new_selected_index];
        }

        pos.y += EditorGUIUtility.singleLineHeight;
        pos.height = position.height - EditorGUIUtility.singleLineHeight;
        _ = EditorGUI.PropertyField(pos, effect_prop, true);
    }

    private void InitEffectTypeNames()
    {
        effectTypeNames = System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(IEffect).IsAssignableFrom(type) && !type.IsInterface)
            .Select(t => t.Name)
            .Select(t => t[..1].ToLower() + t[1..])
            .ToArray();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty effect_type_prop = property.FindPropertyRelative("effectType");
        string effect_type = effect_type_prop.stringValue;
        float height = 0.0f;
        if (effect_type != "")
        {
            height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative(effect_type));
        }
        return EditorGUIUtility.singleLineHeight + height;
    }
}
#endif
