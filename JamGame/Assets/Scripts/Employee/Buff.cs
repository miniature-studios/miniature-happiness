using Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace Employee
{
    [CreateAssetMenu(fileName = "Buff", menuName = "Employee/Buff", order = 1)]
    public class Buff : ScriptableObject
    {
        // TODO: refactor
        public float Time;

        [SerializeField]
        private List<SerializedEffect> rawEffects;
        private List<IEffect> effects;
        public ImmutableList<IEffect> Effects
        {
            get
            {
                if (effects == null)
                {
                    effects = new();
                    foreach (SerializedEffect effect in rawEffects)
                    {
                        effects.Add(effect.ToEffect());
                    }
                }
                return effects.ToImmutableList();
            }
        }

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

    [InterfaceEditor]
    public interface IEffect { }

    [Serializable]
    public class StressEffect : IEffect
    {
        [SerializeField]
        private float increaseMultiplier;
        public float IncreaseMultiplier => increaseMultiplier;
    }

    [Serializable]
    public class NeedModifierEffect : IEffect
    {
        [SerializeField]
        private List<Need.NeedProperties> needModifiers;
        public ImmutableList<Need.NeedProperties> NeedModifiers => needModifiers.ToImmutableList();
    }

    [Serializable]
    public class ControllerEffect : IEffect
    {
        [SerializeField]
        private float speedMultiplier;
        public float SpeedMultiplier => speedMultiplier;
    }
}
