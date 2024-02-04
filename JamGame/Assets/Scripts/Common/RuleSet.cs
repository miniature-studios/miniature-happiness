using System;
using System.Collections.Generic;

namespace Common
{
    public struct Match<T, R>
        where T : Enum
    {
        public T Target;
        public R Result;

        public bool Pass(T match)
        {
            return match.HasFlag(Target);
        }
    }

    public struct RuleSet<T, R>
        where T : Enum
    {
        public List<Match<T, R>> Matches;
        public R DefaultResult;

        public R Apply(T value)
        {
            foreach (Match<T, R> match in Matches)
            {
                if (match.Pass(value))
                {
                    return match.Result;
                }
            }
            return DefaultResult;
        }
    }
}
