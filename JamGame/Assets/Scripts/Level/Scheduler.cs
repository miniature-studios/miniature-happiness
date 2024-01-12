using System.Collections.Generic;
using System.Linq;
using Level.Config;
using UnityEngine;

namespace Level
{
    [AddComponentMenu("Scripts/Level/Level.Scheduler")]
    public class Scheduler : MonoBehaviour
    {
        [SerializeField]
        private ConfigHandler levelConfig;

        [SerializeField]
        private Executor levelExecutor;

        private IEnumerator<DayConfig> dayEnumerator;
        private IEnumerator<IDayAction> actionEnumerator;
        private bool isPlanned = true;

        // Called by Executor when action ends.
        public void ActionEndActivation()
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
            if (levelConfig.Config.Days.Count() > 0)
            {
                _ = (dayEnumerator = levelConfig.Config.Days.GetEnumerator()).MoveNext();
                _ = (
                    actionEnumerator = dayEnumerator.Current.DayActions.GetEnumerator()
                ).MoveNext();
                actionEnumerator.Current.Execute(levelExecutor);
            }
            else
            {
                _ = (
                    actionEnumerator = levelConfig.Config.DefaultDay.DayActions.GetEnumerator()
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
                        actionEnumerator = levelConfig.Config.DefaultDay.DayActions.GetEnumerator()
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
