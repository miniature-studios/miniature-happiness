using System;
using System.Collections;
using UnityEngine;

public partial class LevelExecutor
{
    public void Execute(DayStart day_start, Action next_action)
    {
        financesController.AddMoney(day_start.MorningMoney);
        transitionPanel.SetText("Day start start.");
        uIController.PlayDayActionStart(
            day_start.GetType(),
            () => _ = StartCoroutine(DayStartRoutine(1, next_action))
        );
    }

    private IEnumerator DayStartRoutine(float time, Action next_action)
    {
        yield return new WaitForSeconds(time);
        transitionPanel.SetText("Day start end.");
        uIController.PlayDayActionEnd(next_action);
    }
}
