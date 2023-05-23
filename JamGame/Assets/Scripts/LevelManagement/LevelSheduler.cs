using System.Collections.Generic;
using UnityEngine;

public class LevelSheduler : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private LevelExecutor levelExecuter;

    private IEnumerator<DayConfig> dayEnumerator;
    private IEnumerator<IDayAction> actionEnumerator;

    private void Start()
    {
        if (levelConfig.Days.Count > 0)
        {
            dayEnumerator = levelConfig.Days.GetEnumerator();
            actionEnumerator = dayEnumerator.Current.DayActions.GetEnumerator();
            actionEnumerator.Current.ActionEnd = PlayPlannedActions;
        }
        else
        {
            actionEnumerator = levelConfig.DefaultDay.DayActions.GetEnumerator();
            actionEnumerator.Current.ActionEnd = PlayDefaultDay;
        }
        levelExecuter.ExecuteDayAction(actionEnumerator.Current);
    }

    public void PlayPlannedActions()
    {
        if (!actionEnumerator.MoveNext())
        {
            if (dayEnumerator.MoveNext())
            {
                actionEnumerator = dayEnumerator.Current.DayActions.GetEnumerator();
                actionEnumerator.Current.ActionEnd = PlayPlannedActions;
            }
            else
            {
                actionEnumerator = levelConfig.DefaultDay.DayActions.GetEnumerator();
                actionEnumerator.Current.ActionEnd = PlayDefaultDay;
            }
        }
        levelExecuter.ExecuteDayAction(actionEnumerator.Current);
    }

    public void PlayDefaultDay()
    {
        if (!actionEnumerator.MoveNext())
        {
            actionEnumerator.Reset();
        }
        actionEnumerator.Current.ActionEnd = PlayDefaultDay;
        levelExecuter.ExecuteDayAction(actionEnumerator.Current);
    }
}
