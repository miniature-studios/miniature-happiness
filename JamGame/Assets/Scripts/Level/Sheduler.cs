using Level.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    [AddComponentMenu("Level.Sheduler")]
    public class Sheduler : MonoBehaviour
    {
        [SerializeField]
        private LevelConfig levelActionsConfig;

        [SerializeField]
        private Executor levelExecutor;

        private IEnumerator<DayConfig> dayEnumerator;
        private IEnumerator<IDayAction> actionEnumerator;
        private bool isPlanned = true;

        // Must be catched by Executor
        public void ActionEndRealese()
        {
            if (isPlanned)
            {
                PlayPlannedActions();
            }
            else
            {
                PlayDefaultDay();
            }
        }

        private void Start()
        {
            if (levelActionsConfig.Days.Count > 0)
            {
                _ = (dayEnumerator = levelActionsConfig.Days.GetEnumerator()).MoveNext();
                _ = (
                    actionEnumerator = dayEnumerator.Current.DayActions.GetEnumerator()
                ).MoveNext();
                actionEnumerator.Current.Execute(levelExecutor);
            }
            else
            {
                _ = (
                    actionEnumerator = levelActionsConfig.DefaultDay.DayActions.GetEnumerator()
                ).MoveNext();
                isPlanned = false;
                actionEnumerator.Current.Execute(levelExecutor);
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
                    actionEnumerator.Current.Execute(levelExecutor);
                }
                else
                {
                    _ = (
                        actionEnumerator = levelActionsConfig.DefaultDay.DayActions.GetEnumerator()
                    ).MoveNext();
                    isPlanned = false;
                    actionEnumerator.Current.Execute(levelExecutor);
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
            actionEnumerator.Current.Execute(levelExecutor);
        }
    }
}
