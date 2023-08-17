using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
    public class ConditionsGate
    {
        private MonoBehaviour initiator;
        private List<Func<bool>> conditions;
        private List<Action> actions;

        public ConditionsGate(MonoBehaviour initiator)
        {
            this.initiator = initiator;
        }

        public void SetGates(List<Func<bool>> conditions, List<Action> actions)
        {
            this.conditions = conditions;
            this.actions = actions;
            _ = initiator.StartCoroutine(CheckConditions());
        }

        private IEnumerator CheckConditions()
        {
            if (conditions.All(x => x.Invoke()))
            {
                conditions.Clear();
                List<Action> bufferActions = new(actions);
                actions.Clear();
                foreach (Action action in bufferActions)
                {
                    action.Invoke();
                }
            }
            else
            {
                yield return new WaitForEndOfFrame();
                _ = initiator.StartCoroutine(CheckConditions());
            }
        }
    }
}
