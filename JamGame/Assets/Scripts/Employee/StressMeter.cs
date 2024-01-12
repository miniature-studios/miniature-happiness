using System;
using System.Collections.Generic;
using Employee.Needs;
using Sirenix.OdinInspector;
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
        public StressByNeedDissatisfaction DesatisfactionConfig;
    }

    [Serializable]
    internal struct StressStage
    {
        public float StartsAt;
        public Buff Buff;
    }

    [RequireComponent(typeof(EmployeeImpl))]
    [AddComponentMenu("Scripts/Employee/Employee.StressMeter")]
    public class StressMeter : MonoBehaviour, IEffectExecutor<StressEffect>
    {
        private EmployeeImpl employee;

        [SerializeField]
        private List<StressStage> stages;
        private int currentStage = -1;
        private Buff currentBuff;

        [SerializeField]
        private List<StressByNeedDissatisfactionWithNeedType> configRaw;
        private Dictionary<NeedType, StressByNeedDissatisfaction> config;

        [ReadOnly]
        [SerializeField]
        private float stress = 0.0f;
        public float Value => stress;

        [SerializeField]
        private float restoreSpeed;

        private void OnValidate()
        {
            PrepareConfig();
        }

        private void Start()
        {
            PrepareConfig();

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

            stress += (delta - restoreSpeed) * delta_time;

            int new_stage = 0;
            for (int i = stages.Count - 1; i > 0; i--)
            {
                if (stages[i].StartsAt < stress)
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

        private void PrepareConfig()
        {
            config = new Dictionary<NeedType, StressByNeedDissatisfaction>();
            foreach (StressByNeedDissatisfactionWithNeedType des in configRaw)
            {
                config.Add(des.NeedType, des.DesatisfactionConfig);
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
