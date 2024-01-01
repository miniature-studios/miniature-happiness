using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
    public static class DebugTools
    {
        public static void LogCollection<T>(IEnumerable<T> collection)
        {
            Debug.Log(
                collection.Select(x => x.ToString()).Aggregate("[ ", (x, y) => x + ", " + y) + " ]"
            );
        }
    }
}
