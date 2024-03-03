using System;
using System.Collections.Generic;
using Common;
using Employee.Needs;
using Level.GlobalTime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee
{
    [CreateAssetMenu(fileName = "Buff", menuName = "Employee/Buff", order = 1)]
    public class Buff : ScriptableObject
    {
        [SerializeField]
        [InlineProperty]
        private InternalUid uid;
        public InternalUid Uid => uid;

        public Days Time;

        [SerializeReference]
        private List<IEffect> effects = new();
        public IEnumerable<IEffect> Effects => effects;
    }

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
        [FoldoutGroup("Stress Effect")]
        private float increaseMultiplier;
        public float IncreaseMultiplier => increaseMultiplier;
    }

    [Serializable]
    public class NeedModifierEffect : IEffect
    {
        [SerializeField]
        [FoldoutGroup("NeedModifier Effect")]
        private List<Need.NeedProperties> needModifiers = new();
        public IEnumerable<Need.NeedProperties> NeedModifiers => needModifiers;
    }

    [Serializable]
    public class ControllerEffect : IEffect
    {
        [SerializeField]
        [FoldoutGroup("Controller Effect")]
        private float speedMultiplier;
        public float SpeedMultiplier => speedMultiplier;
    }
}
