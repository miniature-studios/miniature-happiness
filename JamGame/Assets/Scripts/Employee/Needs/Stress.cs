using System;
using System.Collections.Generic;
using UnityEngine;

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
}

public class Stress : MonoBehaviour, IEffectExecutor<StressEffect>
{
    [SerializeField] private List<StressStage> stages;
    private int currentStage = 0;

    [SerializeField] private List<StressByNeedDissatisfactionWithNeedType> configRaw;
    private Dictionary<NeedType, StressByNeedDissatisfaction> config;

    [InspectorReadOnly]
    [SerializeField]
    private float stress = 0.0f;

    [SerializeField] private float restoreSpeed;

    private void OnValidate()
    {
        PrepareConfig();
    }

    private void Start()
    {
        PrepareConfig();
    }

    public void UpdateStress(List<Need> needs, float delta_time)
    {
        float delta = 0.0f;
        foreach (Need need in needs)
        {
            if (config.TryGetValue(need.NeedType, out StressByNeedDissatisfaction diss))
            {
                if (need.satisfied < diss.Threshold)
                {
                    delta += diss.Speed;
                }
            }
        }

        delta *= increaseMultiplierByEffects;

        stress += (delta - restoreSpeed) * delta_time;

        for (int i = 0; i < stages.Count - 1; i++)
        {
            if (stages[i + 1].StartsAt > stress)
            {
                currentStage = i;
                break;
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
