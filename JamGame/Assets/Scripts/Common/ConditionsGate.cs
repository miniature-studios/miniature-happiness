using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
    public static class ConditionsGate
    {
        public static void CreateGate(
            this MonoBehaviour initiator,
            List<Func<bool>> conditions,
            List<Action> actions
        )
        {
            _ = initiator.StartCoroutine(CheckConditions(initiator, conditions, actions));
        }

        private static IEnumerator CheckConditions(
            MonoBehaviour initiator,
            List<Func<bool>> conditions,
            List<Action> actions
        )
        {
            if (conditions.All(x => x.Invoke()))
            {
                foreach (Action action in actions)
                {
                    action.Invoke();
                }
            }
            else
            {
                yield return new WaitForEndOfFrame();
                _ = initiator.StartCoroutine(CheckConditions(initiator, conditions, actions));
            }
        }
    }
}
