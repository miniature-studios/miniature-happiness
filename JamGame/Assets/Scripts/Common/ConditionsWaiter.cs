using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class ConditionsWaiter
    {
        private List<Func<bool>> conditions;
        private List<Action> actions;

        public ConditionsWaiter(List<Func<bool>> conditions, List<Action> actions)
        {
            this.conditions = conditions;
            this.actions = actions;
        }

        public void CheckConditions()
        {
            if (!conditions.Any(x => !x.Invoke()))
            {
                foreach (Action action in actions)
                {
                    action.Invoke();
                }
                conditions.Clear();
                actions.Clear();
            }
        }
    }
}
