using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Employee
{
    [CreateAssetMenu(fileName = "Buff", menuName = "Employee/Buff", order = 1)]
    public class Buff : SerializedScriptableObject
    {
        // TODO: refactor
        public float Time;

        [OdinSerialize]
        public IEnumerable<IEffect> Effects { get; private set; } = new List<IEffect>();

        // TODO: Move to BuffView
        // TODO: Change to Image
        [SerializeField]
        private string name_;
        public string Name => name_;
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
        [OdinSerialize]
        [FoldoutGroup("Stress Effect")]
        public float IncreaseMultiplier { get; private set; }
    }

    [Serializable]
    public class NeedModifierEffect : IEffect
    {
        [OdinSerialize]
        [FoldoutGroup("NeedModifier Effect")]
        public IEnumerable<Need.NeedProperties> NeedModifiers { get; private set; } =
            new List<Need.NeedProperties>();
    }

    [Serializable]
    public class ControllerEffect : IEffect
    {
        [OdinSerialize]
        [FoldoutGroup("Controller Effect")]
        public float SpeedMultiplier { get; private set; }
    }
}
