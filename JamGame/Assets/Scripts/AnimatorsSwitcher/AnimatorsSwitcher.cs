﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorsSwitcher
{
    [Serializable]
    public class AnimatorWithBool
    {
        public string AnimatorName;
        public bool Bool;

        public AnimatorWithBool(string animator, bool _bool)
        {
            AnimatorName = animator;
            Bool = _bool;
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

        public void SetAnimatorStates(Type day_type)
        {
            interfaceMatch = animatorList.InterfaceMatcher.Find(
                x => x.InterfaceName == day_type.Name
            );
            foreach (AnimatorWithBool anim_bools in interfaceMatch.Bools)
            {
                animatorList.Animators
                    .Find(x => x.name == anim_bools.AnimatorName)
                    .SetBool("Showed", anim_bools.Bool);
            }
        }
    }
}