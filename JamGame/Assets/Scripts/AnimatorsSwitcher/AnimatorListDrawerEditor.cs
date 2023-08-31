﻿#if UNITY_EDITOR
using Level.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AnimatorsSwitcher
{
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
                property.FindPropertyRelative("Animators").isExpanded,
                property.FindPropertyRelative("InterfaceMatcher").isExpanded
            ) switch
            {
                (false, false) => EditorGUIUtility.singleLineHeight * 4,
                (true, false) => namedAnimatorsListLength + (EditorGUIUtility.singleLineHeight * 2),
                (false, true)
                    => interfaceMatcherListLength + (EditorGUIUtility.singleLineHeight * 2),
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

            ReorderableList animatorsList = GetAnimatorsList(
                property,
                property.FindPropertyRelative("Animators")
            );
            namedAnimatorsListLength = animatorsList.GetHeight();
            animatorsList.DoList(position);

            ReorderableList interfaceMatcherList = GetInterfaceMatcherList(
                property,
                property.FindPropertyRelative("InterfaceMatcher")
            );
            interfaceMatcherListLength = interfaceMatcherList.GetHeight();
            interfaceMatcherList.DoList(
                new Rect(
                    position.x,
                    position.y + animatorsList.GetHeight(),
                    position.width,
                    position.height
                )
            );

            EditorGUI.indentLevel = indent;

            _ = property.serializedObject.ApplyModifiedProperties();

            EditorGUI.EndProperty();

            List<string> animatorsNames = new();
            for (int x = 0; x < property.FindPropertyRelative("Animators").arraySize; x++)
            {
                SerializedProperty arrayValue = property
                    .FindPropertyRelative("Animators")
                    .GetArrayElementAtIndex(x);
                animatorsNames.Add(
                    (arrayValue.objectReferenceValue as Animator) != null
                        ? (arrayValue.objectReferenceValue as Animator).name
                        : "Unknown animator"
                );
            }

            List<InterfaceMatch> bufferInterfaceMatcher = new();

            SerializedProperty interfaceMatcherArray = property
                .FindPropertyRelative("InterfaceMatcher")
                .Copy();
            for (int i = 0; i < interfaceMatcherArray.arraySize; i++)
            {
                bufferInterfaceMatcher.Add(
                    new(
                        interfaceMatcherArray
                            .GetArrayElementAtIndex(i)
                            .FindPropertyRelative("InterfaceName")
                            .stringValue,
                        new()
                    )
                );
                SerializedProperty array = interfaceMatcherArray
                    .GetArrayElementAtIndex(i)
                    .FindPropertyRelative("Bools");
                for (int j = 0; j < array.arraySize; j++)
                {
                    SerializedProperty arrayElement = array.GetArrayElementAtIndex(j);
                    bufferInterfaceMatcher
                        .Last()
                        .Bools.Add(
                            new(
                                arrayElement.FindPropertyRelative("AnimatorName").stringValue,
                                arrayElement.FindPropertyRelative("Bool").boolValue,
                                arrayElement.FindPropertyRelative("Hard").boolValue
                            )
                        );
                }
            }

            interfaceMatcherArray.ClearArray();
            for (int i = 0; i < ActionNames.Count(); i++)
            {
                interfaceMatcherArray.InsertArrayElementAtIndex(i);
                interfaceMatcherArray
                    .GetArrayElementAtIndex(i)
                    .FindPropertyRelative("Bools")
                    .ClearArray();
                interfaceMatcherArray
                    .GetArrayElementAtIndex(i)
                    .FindPropertyRelative("InterfaceName")
                    .stringValue = ActionNames[i];
                SerializedProperty animatorBoolsArray = interfaceMatcherArray
                    .GetArrayElementAtIndex(i)
                    .FindPropertyRelative("Bools");

                for (int j = 0; j < animatorsNames.Count(); j++)
                {
                    InterfaceMatch foundedInterfaceMatcher = bufferInterfaceMatcher
                        .Where(x => x.InterfaceName == ActionNames[i])
                        .FirstOrDefault();
                    bool flagBool = false;
                    bool flagHard = false;
                    if (foundedInterfaceMatcher != null)
                    {
                        List<AnimatorWithBool> foundedAnimatorBoolArray =
                            foundedInterfaceMatcher.Bools;
                        for (int k = 0; k < foundedAnimatorBoolArray.Count; k++)
                        {
                            if (foundedAnimatorBoolArray[k].AnimatorName == animatorsNames[j])
                            {
                                flagBool = foundedAnimatorBoolArray[k].Bool;
                                flagHard = foundedAnimatorBoolArray[k].Hard;
                            }
                        }
                    }
                    animatorBoolsArray.InsertArrayElementAtIndex(j);
                    SerializedProperty arrayElement = animatorBoolsArray.GetArrayElementAtIndex(j);
                    arrayElement.FindPropertyRelative("AnimatorName").stringValue = animatorsNames[
                        j
                    ];
                    arrayElement.FindPropertyRelative("Bool").boolValue = flagBool;
                    arrayElement.FindPropertyRelative("Hard").boolValue = flagHard;
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
                            ReorderableList animatorBoolList = GetAnimatorBoolList(
                                element,
                                element.FindPropertyRelative("Bools"),
                                element.FindPropertyRelative("InterfaceName").stringValue
                            );
                            animatorBoolList.DoList(rect);
                        }
                    },
                    elementHeightCallback = (int indexer) =>
                    {
                        SerializedProperty element = listProperty.GetArrayElementAtIndex(indexer);
                        return !listProperty.isExpanded
                            ? 0
                            : GetAnimatorBoolList(
                                    element,
                                    element.FindPropertyRelative("Bools"),
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
            string listName
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
                            listName + $" - {listProperty.arraySize}",
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

                            position.x += length;
                            length = 40;
                            EditorGUI.LabelField(
                                new Rect(
                                    position.x,
                                    position.y,
                                    length,
                                    EditorGUIUtility.singleLineHeight
                                ),
                                "Show:"
                            );

                            position.x += length;
                            length = 20;
                            element.FindPropertyRelative("Bool").boolValue = EditorGUI.Toggle(
                                new Rect(
                                    position.x,
                                    position.y,
                                    length,
                                    EditorGUIUtility.singleLineHeight
                                ),
                                element.FindPropertyRelative("Bool").boolValue
                            );

                            position.x += length;
                            length = 40;
                            EditorGUI.LabelField(
                                new Rect(
                                    position.x,
                                    position.y,
                                    length,
                                    EditorGUIUtility.singleLineHeight
                                ),
                                "Hard:"
                            );

                            position.x += length;
                            length = 20;
                            element.FindPropertyRelative("Hard").boolValue = EditorGUI.Toggle(
                                new Rect(
                                    position.x,
                                    position.y,
                                    length,
                                    EditorGUIUtility.singleLineHeight
                                ),
                                element.FindPropertyRelative("Hard").boolValue
                            );
                        }
                    }
                };

            list.elementHeightCallback = (int indexer) =>
                !listProperty.isExpanded ? 0 : list.elementHeight;

            return list;
        }

        private ReorderableList GetAnimatorsList(
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
                                element.objectReferenceValue != null
                                    ? $"{(element.objectReferenceValue as Animator).name}: "
                                    : "Unknown animator: ";
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

                            position.x += length + 10;
                            length = 300;

                            element.objectReferenceValue = EditorGUI.ObjectField(
                                new Rect(
                                    position.x,
                                    position.y,
                                    length,
                                    EditorGUIUtility.singleLineHeight
                                ),
                                element.objectReferenceValue,
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
}
#endif
