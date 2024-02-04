using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Common
{
    public enum MatchType
    {
        Includes,
        Excludes
    }

    public struct Match<T, R>
        where T : Enum
    {
        public T Target;
        public R Result;
        public MatchType MatchType;

        public bool Pass(T match)
        {
            return MatchType switch
            {
                MatchType.Includes => match.HasFlag(Target),
                MatchType.Excludes
                    => (Unsafe.As<T, int>(ref Target) & Unsafe.As<T, int>(ref match)) == 0,
                _ => throw new ArgumentException()
            };
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
