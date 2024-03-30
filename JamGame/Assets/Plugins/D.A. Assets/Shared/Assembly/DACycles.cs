using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DA_Assets.Shared
{
    public class DACycles
    {
        public static IEnumerator ForEach<T>(IList<T> source, Action<T> body, float iterationTimeout = 0, int beforeWaitItersCount = 0)
        {
            if (source.IsEmpty())
            {
                yield break;
            }

            for (int i = 0; i < source.Count; i++)
            {
                if (i != 0 &&
                    iterationTimeout != 0 &&
                    beforeWaitItersCount != 0 &&
                    i % beforeWaitItersCount == 0)
                {
                    yield return WaitFor.Delay(iterationTimeout);
                }

                body.Invoke(source[i]);
            }
        }
    }
}