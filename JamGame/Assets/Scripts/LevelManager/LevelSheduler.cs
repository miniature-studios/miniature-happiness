using System.Collections.Generic;
using UnityEngine;

public class LevelSheduler : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private LevelExecuter levelExecuter;
    private readonly List<DayAction> allActions = new();
    private IEnumerator<DayAction> actionEnumerator;
    private void Start()
    {
        levelExecuter.SetTarrifs(levelConfig.tariffs);
        foreach (DayConfig day in levelConfig.Days)
        {
            allActions.AddRange(day.DayActions);
        }
        if (allActions.Count > 0)
        {
            actionEnumerator = allActions.GetEnumerator();
            actionEnumerator.Current.ReleaseAction(levelExecuter, () => PlayPlannedActions());
        }
        else
        {
            PlayDefaultDay();
        }
    }
    public void PlayPlannedActions()
    {
        if (actionEnumerator.MoveNext())
        {
            actionEnumerator.Current.ReleaseAction(levelExecuter, () => PlayPlannedActions());
        }
        else
        {
            actionEnumerator = levelConfig.DefaultDay.DayActions.GetEnumerator();
            actionEnumerator.Current.ReleaseAction(levelExecuter, () => PlayDefaultDay());
        }
    }
    public void PlayDefaultDay()
    {
        if (!actionEnumerator.MoveNext())
        {
            actionEnumerator.Reset();
        }
        actionEnumerator.Current.ReleaseAction(levelExecuter, () => PlayDefaultDay());
    }
}
