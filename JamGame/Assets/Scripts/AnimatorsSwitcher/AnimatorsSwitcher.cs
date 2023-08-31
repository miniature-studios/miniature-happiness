using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorsSwitcher
{
    [Serializable]
    public class AnimatorWithBool
    {
        public string AnimatorName;
        public bool Bool;
        public bool Hard;

        public AnimatorWithBool(string animator, bool flag, bool hard)
        {
            AnimatorName = animator;
            Bool = flag;
            Hard = hard;
        }
    }

    [Serializable]
    public class InterfaceMatch
    {
        public string InterfaceName;
        public List<AnimatorWithBool> Bools;

        public InterfaceMatch(string interfaceName, List<AnimatorWithBool> bools)
        {
            InterfaceName = interfaceName;
            Bools = bools;
        }
    }

    [Serializable]
    public class AnimatorList
    {
        public List<Animator> Animators = new();
        public List<InterfaceMatch> InterfaceMatcher = new();
    }

    public class AnimatorsSwitcher : MonoBehaviour
    {
        [SerializeField]
        private AnimatorList animatorList;
        private InterfaceMatch interfaceMatch;

        public void SetAnimatorStates(Type dayType)
        {
            interfaceMatch = animatorList.InterfaceMatcher.Find(
                x => x.InterfaceName == dayType.Name
            );
            foreach (AnimatorWithBool bools in interfaceMatch.Bools)
            {
                Animator animator = animatorList.Animators.Find(x => x.name == bools.AnimatorName);
                animator.SetBool("Showed", bools.Bool);
                if (bools.Hard)
                {
                    animator.Play(bools.Bool ? "Showed" : "Hidden");
                }
            }
        }
    }
}
