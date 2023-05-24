using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[Serializable]
public class Tariffs
{
    [SerializeField] private float waterCost;
    [SerializeField] private float electricityCost;
    public float WaterCost => waterCost;
    public float ElectricityCost => electricityCost;
}

[Serializable]
public class DayConfig
{
    [SerializeField] private List<SerializedDayAction> rawDayActions;
    private List<IDayAction> dayActions;
    public ReadOnlyCollection<IDayAction> DayActions
    {
        get
        {
            if (dayActions == null)
            {
                dayActions = new();
                dayActions.AddRange(rawDayActions.Select(x => x.ToDayAction()));
            }
            return dayActions.AsReadOnly();
        }
    }
}

[Serializable]
[CreateAssetMenu(fileName = "LevelConfig", menuName = "Level/LevelConfig", order = 0)]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private float bossStressSpeed;
    [SerializeField] private Tariffs tariffs;
    [SerializeField] private List<DayConfig> days;
    [SerializeField] private DayConfig defaultDay;

    public float BossStressSpeed => bossStressSpeed;
    public Tariffs Tariffs => tariffs;
    public ReadOnlyCollection<DayConfig> Days => days.AsReadOnly();
    public DayConfig DefaultDay => defaultDay;
}
