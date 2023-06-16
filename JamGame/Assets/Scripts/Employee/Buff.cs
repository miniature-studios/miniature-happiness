using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ReadOnlyCollection<IEffect> Effects
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

                return effects.AsReadOnly();
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
    public class Stress : IEffect
    {
        [SerializeField]
        private float increaseMultiplier;
        public float IncreaseMultiplier => increaseMultiplier;
    }

    [Serializable]
    public class NeedModifier : IEffect
    {
        [SerializeField]
        private List<Need.NeedProperties> needModifiers;
        public ReadOnlyCollection<Need.NeedProperties> NeedModifiers => needModifiers.AsReadOnly();
    }

    [Serializable]
    public class Controller : IEffect
    {
        [SerializeField]
        private float speedMultiplier;
        public float SpeedMultiplier => speedMultiplier;
    }
}
