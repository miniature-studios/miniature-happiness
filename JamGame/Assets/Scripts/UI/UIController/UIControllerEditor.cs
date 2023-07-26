#if UNITY_EDITOR
using Level.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimatorList))]
public class AnimatorListDrawer : PropertyDrawer
{
    private string[] ActionNames =>
        Assembly
            .GetAssembly(interfaceType)
            .GetTypes()
            .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface)
            .Select(t => t.Name)
            .ToArray();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (
            property.FindPropertyRelative("NamedAnimators").isExpanded,
            property.FindPropertyRelative("InterfaceMatcher").isExpanded
        ) switch
        {
            (false, false) => EditorGUIUtility.singleLineHeight * 4,
            (true, false) => namedAnimatorsListLength + (EditorGUIUtility.singleLineHeight * 2),
            (false, true) => interfaceMatcherListLength + (EditorGUIUtility.singleLineHeight * 2),
            (true, true)
                => namedAnimatorsListLength
                    + interfaceMatcherListLength
                    - EditorGUIUtility.singleLineHeight,
        };
    }

    private float namedAnimatorsListLength;
    private float interfaceMatcherListLength;
    private readonly Type interfaceType = typeof(IDayAction);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _ = EditorGUI.BeginProperty(position, label, property);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        ReorderableList named_animators_list = GetNamedAnimatorsList(
            property,
            property.FindPropertyRelative("NamedAnimators")
        );
        namedAnimatorsListLength = named_animators_list.GetHeight();
        named_animators_list.DoList(position);

        ReorderableList interface_matcher_list = GetInterfaceMatcherList(
            property,
            property.FindPropertyRelative("InterfaceMatcher")
        );
        interfaceMatcherListLength = interface_matcher_list.GetHeight();
        interface_matcher_list.DoList(
            new Rect(
                position.x,
                position.y + named_animators_list.GetHeight(),
                position.width,
                position.height
            )
        );

        EditorGUI.indentLevel = indent;

        _ = property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();

        List<string> animatorsNames = new();
        for (int x = 0; x < property.FindPropertyRelative("NamedAnimators").arraySize; x++)
        {
            SerializedProperty array_value = property
                .FindPropertyRelative("NamedAnimators")
                .GetArrayElementAtIndex(x);
            animatorsNames.Add(array_value.FindPropertyRelative("Name").stringValue);
        }

        List<InterfaceMatch> buffer_interface_matcher = new();

        SerializedProperty interface_matcher_array = property
            .FindPropertyRelative("InterfaceMatcher")
            .Copy();
        for (int i = 0; i < interface_matcher_array.arraySize; i++)
        {
            buffer_interface_matcher.Add(
                new(
                    interface_matcher_array
                        .GetArrayElementAtIndex(i)
                        .FindPropertyRelative("InterfaceName")
                        .stringValue,
                    new()
                )
            );
            SerializedProperty _array = interface_matcher_array
                .GetArrayElementAtIndex(i)
                .FindPropertyRelative("AnimatorBools");
            for (int j = 0; j < _array.arraySize; j++)
            {
                SerializedProperty _arr_element = _array.GetArrayElementAtIndex(j);
                buffer_interface_matcher
                    .Last()
                    .AnimatorBools.Add(
                        new(
                            _arr_element.FindPropertyRelative("AnimatorName").stringValue,
                            _arr_element.FindPropertyRelative("StartDelay").floatValue,
                            _arr_element.FindPropertyRelative("StartIsActive").boolValue,
                            _arr_element.FindPropertyRelative("EndDelay").floatValue,
                            _arr_element.FindPropertyRelative("EndIsActive").boolValue
                        )
                    );
            }
        }
        interface_matcher_array.ClearArray();
        for (int i = 0; i < ActionNames.Count(); i++)
        {
            interface_matcher_array.InsertArrayElementAtIndex(i);
            interface_matcher_array
                .GetArrayElementAtIndex(i)
                .FindPropertyRelative("AnimatorBools")
                .ClearArray();
            interface_matcher_array
                .GetArrayElementAtIndex(i)
                .FindPropertyRelative("InterfaceName")
                .stringValue = ActionNames[i];
            SerializedProperty animator_bools_array = interface_matcher_array
                .GetArrayElementAtIndex(i)
                .FindPropertyRelative("AnimatorBools");

            for (int j = 0; j < animatorsNames.Count(); j++)
            {
                InterfaceMatch founded_interface_matcher = buffer_interface_matcher
                    .Where(x => x.InterfaceName == ActionNames[i])
                    .FirstOrDefault();
                float start_delay = 0;
                bool start_is_active = false;
                float end_delay = 0;
                bool end_is_active = false;
                if (founded_interface_matcher != null)
                {
                    List<AnimatorBool> founded_animator_bool_array =
                        founded_interface_matcher.AnimatorBools;
                    for (int k = 0; k < founded_animator_bool_array.Count; k++)
                    {
                        if (founded_animator_bool_array[k].AnimatorName == animatorsNames[j])
                        {
                            start_delay = founded_animator_bool_array[k].StartDelay;
                            start_is_active = founded_animator_bool_array[k].StartIsActive;
                            end_delay = founded_animator_bool_array[k].EndDelay;
                            end_is_active = founded_animator_bool_array[k].EndIsActive;
                        }
                    }
                }
                animator_bools_array.InsertArrayElementAtIndex(j);
                SerializedProperty _arr_element = animator_bools_array.GetArrayElementAtIndex(j);
                _arr_element.FindPropertyRelative("AnimatorName").stringValue = animatorsNames[j];
                _arr_element.FindPropertyRelative("StartDelay").floatValue = start_delay;
                _arr_element.FindPropertyRelative("StartIsActive").boolValue = start_is_active;
                _arr_element.FindPropertyRelative("EndDelay").floatValue = end_delay;
                _arr_element.FindPropertyRelative("EndIsActive").boolValue = end_is_active;
            }
        }
    }

    private ReorderableList GetInterfaceMatcherList(
        SerializedProperty property,
        SerializedProperty listProperty
    )
    {
        ReorderableList list =
            new(property.serializedObject, listProperty, false, true, false, false)
            {
                drawHeaderCallback = rect =>
                {
                    Rect newRect = new(rect.x + 10, rect.y, rect.width - 10, rect.height);
                    listProperty.isExpanded = EditorGUI.Foldout(
                        newRect,
                        listProperty.isExpanded,
                        listProperty.displayName
                            + $" - {typeof(IDayAction).Name}"
                            + $" - {listProperty.arraySize}",
                        true
                    );
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (listProperty.isExpanded)
                    {
                        SerializedProperty element = listProperty.GetArrayElementAtIndex(index);
                        ReorderableList animator_bool_list = GetAnimatorBoolList(
                            element,
                            element.FindPropertyRelative("AnimatorBools"),
                            element.FindPropertyRelative("InterfaceName").stringValue
                        );
                        animator_bool_list.DoList(rect);
                    }
                },
                elementHeightCallback = (int indexer) =>
                {
                    SerializedProperty element = listProperty.GetArrayElementAtIndex(indexer);
                    return !listProperty.isExpanded
                        ? 0
                        : GetAnimatorBoolList(
                                element,
                                element.FindPropertyRelative("AnimatorBools"),
                                element.FindPropertyRelative("InterfaceName").stringValue
                            )
                            .GetHeight() - EditorGUIUtility.singleLineHeight;
                }
            };
        return list;
    }

    private ReorderableList GetAnimatorBoolList(
        SerializedProperty property,
        SerializedProperty listProperty,
        string list_name
    )
    {
        ReorderableList list =
            new(property.serializedObject, listProperty, false, true, false, false)
            {
                elementHeight = EditorGUIUtility.singleLineHeight,
                drawHeaderCallback = rect =>
                {
                    Rect newRect = new(rect.x + 10, rect.y, rect.width - 10, rect.height);
                    listProperty.isExpanded = EditorGUI.Foldout(
                        newRect,
                        listProperty.isExpanded,
                        list_name + $" - {listProperty.arraySize}",
                        true
                    );
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (listProperty.isExpanded)
                    {
                        SerializedProperty element = listProperty.GetArrayElementAtIndex(index);

                        Vector2 position = rect.position;
                        float length = 150;

                        string text = element.FindPropertyRelative("AnimatorName").stringValue;
                        EditorGUI.LabelField(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            text
                        );

                        text = "On start: Showed ";
                        position.x += length;
                        length = GUI.skin.label.CalcSize(new GUIContent(text)).x;
                        EditorGUI.LabelField(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            text
                        );

                        position.x += length;
                        length = 20;
                        element.FindPropertyRelative("StartIsActive").boolValue = EditorGUI.Toggle(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            element.FindPropertyRelative("StartIsActive").boolValue
                        );

                        text = "Delay ";
                        position.x += length;
                        length = GUI.skin.label.CalcSize(new GUIContent(text)).x;
                        EditorGUI.LabelField(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            text
                        );

                        position.x += length;
                        length = 30;
                        element.FindPropertyRelative("StartDelay").floatValue =
                            EditorGUI.FloatField(
                                new Rect(
                                    position.x,
                                    position.y,
                                    length,
                                    EditorGUIUtility.singleLineHeight
                                ),
                                element.FindPropertyRelative("StartDelay").floatValue
                            );

                        text = "On end: Showed ";
                        position.x += length + 10;
                        length = GUI.skin.label.CalcSize(new GUIContent(text)).x;
                        EditorGUI.LabelField(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            text
                        );

                        position.x += length;
                        length = 20;
                        element.FindPropertyRelative("EndIsActive").boolValue = EditorGUI.Toggle(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            element.FindPropertyRelative("EndIsActive").boolValue
                        );

                        text = "Delay ";
                        position.x += length;
                        length = GUI.skin.label.CalcSize(new GUIContent(text)).x;
                        EditorGUI.LabelField(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            text
                        );

                        position.x += length;
                        length = 30;
                        element.FindPropertyRelative("EndDelay").floatValue = EditorGUI.FloatField(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            element.FindPropertyRelative("EndDelay").floatValue
                        );
                    }
                }
            };

        list.elementHeightCallback = (int indexer) =>
            !listProperty.isExpanded ? 0 : list.elementHeight;

        return list;
    }

    private ReorderableList GetNamedAnimatorsList(
        SerializedProperty property,
        SerializedProperty listProperty
    )
    {
        ReorderableList list =
            new(property.serializedObject, listProperty, false, true, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight,
                drawHeaderCallback = rect =>
                {
                    Rect newRect = new(rect.x + 10, rect.y, rect.width - 10, rect.height);
                    listProperty.isExpanded = EditorGUI.Foldout(
                        newRect,
                        listProperty.isExpanded,
                        listProperty.displayName + $" - {listProperty.arraySize}",
                        true
                    );
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (listProperty.isExpanded)
                    {
                        SerializedProperty element = listProperty.GetArrayElementAtIndex(index);

                        Vector2 position = rect.position;
                        string text =
                            $"{(element.FindPropertyRelative("Animator").objectReferenceValue as Animator).name}: ";
                        float length = 110;
                        EditorGUI.LabelField(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            text
                        );

                        position.x += length;
                        length = 150;

                        element.FindPropertyRelative("Name").stringValue = EditorGUI.TextField(
                            new Rect(
                                position.x,
                                position.y,
                                length,
                                EditorGUIUtility.singleLineHeight
                            ),
                            element.FindPropertyRelative("Name").stringValue
                        );

                        position.x += length + 10;
                        length = 300;

                        element.FindPropertyRelative("Animator").objectReferenceValue =
                            EditorGUI.ObjectField(
                                new Rect(
                                    position.x,
                                    position.y,
                                    length,
                                    EditorGUIUtility.singleLineHeight
                                ),
                                element.FindPropertyRelative("Animator").objectReferenceValue,
                                typeof(Animator),
                                true
                            );
                    }
                }
            };

        list.elementHeightCallback = (int indexer) =>
            !listProperty.isExpanded ? 0 : list.elementHeight;

        return list;
    }
}
#endif
