﻿using System;
using System.Collections.Generic;
using Employee.Needs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee
{
    public interface IEffectExecutor { }

    public interface IEffectExecutor<E> : IEffectExecutor
        where E : class, IEffect
    {
        public void RegisterEffect(E effect);
        public void UnregisterEffect(E effect);
    }

    [HideReferenceObjectPicker]
    public interface IEffect { }

    [Serializable]
    public class StressEffect : IEffect
    {
        [SerializeField]
        [FoldoutGroup("Stress effect")]
        private float increaseMultiplier;
        public float IncreaseMultiplier => increaseMultiplier;
    }

    [Serializable]
    public class NeedModifierEffect : IEffect
    {
        [SerializeField]
        [FoldoutGroup("NeedModifier effect")]
        private List<Need.NeedProperties> needModifiers = new();
        public IEnumerable<Need.NeedProperties> NeedModifiers => needModifiers;
    }

    [Serializable]
    public class ControllerEffect : IEffect
    {
        [SerializeField]
        [FoldoutGroup("Controller effect")]
        private float speedMultiplier;
        public float SpeedMultiplier => speedMultiplier;
    }

    [Serializable]
    public class EarnedMoneyEffect : IEffect
    {
        [SerializeField]
        [FoldoutGroup("Earned money effect")]
        private float multiplier;
        public float Multiplier => multiplier;
    }
}
