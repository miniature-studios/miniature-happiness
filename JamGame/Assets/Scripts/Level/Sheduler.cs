using Level.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class Sheduler : MonoBehaviour
    {
        [SerializeField]
        private LevelConfig levelActionsConfig;

        [SerializeField]
        private Executor levelExecutor;

        private IEnumerator<DayConfig> dayEnumerator;
        private IEnumerator<IDayAction> actionEnumerator;

        private void Start()
        {
            if (levelActionsConfig.Days.Count > 0)
            {
                _ = (dayEnumerator = levelActionsConfig.Days.GetEnumerator()).MoveNext();
                _ = (
                    actionEnumerator = dayEnumerator.Current.DayActions.GetEnumerator()
                ).MoveNext();
                actionEnumerator.Current.Execute(levelExecutor, PlayPlannedActions);
            }
            else
            {
                _ = (
                    actionEnumerator = levelActionsConfig.DefaultDay.DayActions.GetEnumerator()
                ).MoveNext();
                actionEnumerator.Current.Execute(levelExecutor, PlayDefaultDay);
            }
        }

        private void PlayPlannedActions()
        {
            if (!actionEnumerator.MoveNext())
            {
                if (dayEnumerator.MoveNext())
                {
                    _ = (
                        actionEnumerator = dayEnumerator.Current.DayActions.GetEnumerator()
                    ).MoveNext();
                    actionEnumerator.Current.Execute(levelExecutor, PlayPlannedActions);
                }
                else
                {
                    _ = (
                        actionEnumerator = levelActionsConfig.DefaultDay.DayActions.GetEnumerator()
                    ).MoveNext();
                    actionEnumerator.Current.Execute(levelExecutor, PlayDefaultDay);
                }
            }
        }

        private void PlayDefaultDay()
        {
            if (!actionEnumerator.MoveNext())
            {
                actionEnumerator.Reset();
                _ = actionEnumerator.MoveNext();
            }
            actionEnumerator.Current.Execute(levelExecutor, PlayDefaultDay);
        }
    }
}