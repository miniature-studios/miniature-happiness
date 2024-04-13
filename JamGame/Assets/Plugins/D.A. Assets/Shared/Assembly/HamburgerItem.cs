using System;
using UnityEngine;

namespace DA_Assets.Shared
{
    [Serializable]
    public class HamburgerItem
    {
        [SerializeField] string id;
#if UNITY_EDITOR
        [SerializeField] UnityEditor.AnimatedValues.AnimBool fade;
#endif
        [SerializeField] UpdateBool checkBoxValue = new UpdateBool(false, false);
        [SerializeField] GUIContent guiContent;
        [SerializeField] Action body;
        [SerializeField] Action<string, bool> checkBoxValueChanged;
        [SerializeField] Action onButtonClick;
        [SerializeField] GUIContent buttonGuiContent;

        public string Id { get => id; set => id = value; }
#if UNITY_EDITOR
        public UnityEditor.AnimatedValues.AnimBool Fade { get => fade; set => fade = value; }
#endif
        public UpdateBool CheckBoxValue { get => checkBoxValue; set => checkBoxValue = value; }
        public GUIContent GUIContent { get => guiContent; set => guiContent = value; }
        public Action Body { get => body; set => body = value; }
        public Action<string, bool> CheckBoxValueChanged { get => checkBoxValueChanged; set => checkBoxValueChanged = value; }
        public Action OnButtonClick { get => onButtonClick; set => onButtonClick = value; }
        public GUIContent ButtonGuiContent { get => buttonGuiContent; set => buttonGuiContent = value; }
    }
}
