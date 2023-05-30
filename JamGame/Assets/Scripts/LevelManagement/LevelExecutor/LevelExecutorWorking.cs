using System;
using System.Collections;
using UnityEngine;

public partial class LevelExecutor
{
    public void Execute(Working working, Action next_action)
    {
        transitionPanel.SetText("Working start.");
        uIController.PlayDayActionStart(working.GetType(), () => StartCoroutine(WorkingTime(working.WorkingTime, next_action)));
    }

    private IEnumerator WorkingTime(float time, Action endWorkingTime)
    {
        yield return new WaitForSeconds(time);
        transitionPanel.SetText("Working end.");
        uIController.PlayDayActionEnd(endWorkingTime);
    }
}

