using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[Serializable]
public class DayConfig
{
    [SerializeField]
    private List<SerializedDayAction> rawDayActions;
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
[CreateAssetMenu(fileName = "LevelActionsConfig", menuName = "Level/LevelActionsConfig", order = 0)]
public class LevelActionsConfig : ScriptableObject
{
    [SerializeField]
    private List<DayConfig> days;

    [SerializeField]
    private DayConfig defaultDay;
    public ReadOnlyCollection<DayConfig> Days => days.AsReadOnly();
    public DayConfig DefaultDay => defaultDay;
}
