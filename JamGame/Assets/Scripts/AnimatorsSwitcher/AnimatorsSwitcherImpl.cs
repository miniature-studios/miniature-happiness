using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AnimatorsSwitcher
{
    public enum OverrideState
    {
        DoNotOverride,
        Hidden,
        Showed
    }

    [Serializable]
    public class AnimatorProperties
    {
        [HideInInspector]
        public Animator Animator;
        private string AnimatorName => Animator == null ? "NULL" : Animator.name;

        [LabelText("@" + nameof(AnimatorName))]
        [HorizontalGroup()]
        public bool Showed;

        [HideLabel]
        [HorizontalGroup(Width = 200, LabelWidth = 0, DisableAutomaticLabelWidth = true)]
        public OverrideState OverrideState;
    }

    [Serializable]
    public class InterfaceMatch
    {
        [HideInInspector]
        public string InterfaceName;

        [LabelText("@" + nameof(InterfaceName))]
        [ListDrawerSettings(
            DraggableItems = false,
            HideRemoveButton = true,
            HideAddButton = true,
            IsReadOnly = true
        )]
        public List<AnimatorProperties> AnimatorsProperties;
    }

    [Serializable]
    public partial class AnimatorList
    {
        [ListDrawerSettings(
            OnTitleBarGUI = nameof(TitleBarGUIAnimators),
            CustomRemoveElementFunction = nameof(CustomRemoveElementFunction),
            CustomRemoveIndexFunction = nameof(CustomRemoveIndexFunction)
        )]
        public List<Animator> Animators = new();

        [ListDrawerSettings(
            DraggableItems = false,
            HideRemoveButton = true,
            HideAddButton = true,
            OnTitleBarGUI = nameof(TitleBarGUIInterfaceMatcher)
        )]
        public List<InterfaceMatch> InterfaceMatcher = new();
    }

    [AddComponentMenu("Scripts/AnimatorsSwitcher/AnimatorsSwitcher")]
    public class AnimatorsSwitcherImpl : MonoBehaviour
    {
        [HideLabel]
        [SerializeField]
        [InlineProperty]
        private AnimatorList animatorList;

        public void SetAnimatorStates(Type dayType)
        {
            InterfaceMatch interfaceMatch = animatorList.InterfaceMatcher.Find(x =>
                x.InterfaceName == dayType.Name
            );
            foreach (AnimatorProperties properties in interfaceMatch.AnimatorsProperties)
            {
                properties.Animator.SetBool("Showed", properties.Showed);
                switch (properties.OverrideState)
                {
                    case OverrideState.DoNotOverride:
                        continue;
                    case OverrideState.Hidden:
                        properties.Animator.Play("Hidden");
                        break;
                    case OverrideState.Showed:
                        properties.Animator.Play("Showed");
                        break;
                }
            }
        }
    }
}
