using System;
using System.Collections;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DayStart", menuName = "Level/DayActions/DayStart", order = 0)]
public class DayStart : DayAction
{
    public override void ReleaseAction(LevelExecutor Level_executor, Action show_end_handler)
    {
        _ = Level_executor.StartCoroutine(StartingAnimation(show_end_handler));
    }

    private IEnumerator StartingAnimation(Action end_delegate)
    {
        yield return new WaitForSeconds(3f);
        end_delegate();
    }
}

