using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Employee
{
    [Serializable]
    internal struct StressByNeedDissatisfaction
    {
        public float Threshold;
        public float Speed;
    }

    [Serializable]
    internal struct StressByNeedDissatisfactionWithNeedType
    {
        public NeedType NeedType;
        public StressByNeedDissatisfaction DissatisfactionConfig;
    }

    [Serializable]
    internal struct StressStage
    {
        [FoldoutGroup("@Label")]
        public float StartsAt;

        [FoldoutGroup("@Label")]
        public Buff Buff;

        [Discardable]
        private string Label => Buff?.Name;
    }

    [RequireComponent(typeof(EmployeeImpl))]
    [AddComponentMenu("Scripts/Employee.StressMeter")]
    public class StressMeter : SerializedMonoBehaviour, IEffectExecutor<StressEffect>
    {
        private EmployeeImpl employee;

        [SerializeField]
        private List<StressStage> stages;
        private int currentStage = -1;
        private Buff currentBuff;

        [OdinSerialize]
        [ShowInInspector]
        private Dictionary<NeedType, StressByNeedDissatisfaction> config = new();

        [ReadOnly]
        [OdinSerialize]
        public float StressValue { get; private set; }

        [SerializeField]
        private float restoreSpeed;

        private void Start()
        {
            employee = GetComponent<EmployeeImpl>();
        }

        public void UpdateStress(List<Need> needs, float delta_time)
        {
            float delta = 0.0f;
            foreach (Need need in needs)
            {
                if (config.TryGetValue(need.NeedType, out StressByNeedDissatisfaction diss))
                {
                    if (need.Satisfied < diss.Threshold)
                    {
                        delta += diss.Speed;
                    }
                }
            }

            delta *= increaseMultiplierByEffects;

            StressValue += (delta - restoreSpeed) * delta_time;

            int new_stage = 0;
            for (int i = stages.Count - 1; i > 0; i--)
            {
                if (stages[i].StartsAt < StressValue)
                {
                    new_stage = i;
                    break;
                }
            }

            if (currentStage != new_stage)
            {
                currentStage = new_stage;

                if (currentBuff != null)
                {
                    employee.UnregisterBuff(currentBuff);
                }

                currentBuff = stages[currentStage].Buff;

                if (currentBuff != null)
                {
                    employee.RegisterBuff(currentBuff);
                }
            }
        }

        private float increaseMultiplierByEffects = 1.0f;
        private readonly List<StressEffect> registeredEffects = new();

        public void RegisterEffect(StressEffect effect)
        {
            registeredEffects.Add(effect);
            increaseMultiplierByEffects *= effect.IncreaseMultiplier;
        }

        public void UnregisterEffect(StressEffect effect)
        {
            if (!registeredEffects.Remove(effect))
            {
                Debug.LogError("Failed to remove StressEffect: Not registered");
                return;
            }

            increaseMultiplierByEffects = 1.0f;
            foreach (StressEffect eff in registeredEffects)
            {
                increaseMultiplierByEffects *= eff.IncreaseMultiplier;
            }
        }
    }
}
