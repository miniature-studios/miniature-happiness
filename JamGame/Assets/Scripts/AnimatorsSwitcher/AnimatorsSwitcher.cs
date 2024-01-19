using System;
using System.Collections.Generic;
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
        public string AnimatorName;
        public bool Showed;
        public OverrideState OverrideState;

        public AnimatorProperties(string animator, bool showed, OverrideState overrideState)
        {
            AnimatorName = animator;
            Showed = showed;
            OverrideState = overrideState;
        }
    }

    [Serializable]
    public class InterfaceMatch
    {
        public string InterfaceName;
        public List<AnimatorProperties> AnimatorsProperties;

        public InterfaceMatch(string interfaceName, List<AnimatorProperties> animatorsProperties)
        {
            InterfaceName = interfaceName;
            AnimatorsProperties = animatorsProperties;
        }
    }

    [Serializable]
    public class AnimatorList
    {
        public List<Animator> Animators = new();
        public List<InterfaceMatch> InterfaceMatcher = new();
    }

    [AddComponentMenu("Scripts/AnimatorsSwitcher/AnimatorsSwitcher.AnimatorsSwitcher")]
    public class AnimatorsSwitcher : MonoBehaviour
    {
        [SerializeField]
        private AnimatorList animatorList;
        private InterfaceMatch interfaceMatch;

        public void SetAnimatorStates(Type dayType)
        {
            interfaceMatch = animatorList.InterfaceMatcher.Find(x =>
                x.InterfaceName == dayType.Name
            );
            foreach (AnimatorProperties properties in interfaceMatch.AnimatorsProperties)
            {
                Animator animator = animatorList.Animators.Find(x =>
                    x.name == properties.AnimatorName
                );
                animator.SetBool("Showed", properties.Showed);
                switch (properties.OverrideState)
                {
                    case OverrideState.DoNotOverride:
                        continue;
                    case OverrideState.Hidden:
                        animator.Play("Hidden");
                        break;
                    case OverrideState.Showed:
                        animator.Play("Showed");
                        break;
                }
            }
        }
    }
}
