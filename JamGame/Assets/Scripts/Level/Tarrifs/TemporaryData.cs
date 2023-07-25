using Common;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    // TODO: Remove this.
    [AddComponentMenu("Level.TemporaryData")]
    public class TemporaryData : MonoBehaviour
    {
        [InspectorReadOnly]
        private Check check;

        public UnityEvent<IReadonlyData<Check>> CheckChanged;

        public void CreateCheck(Check new_check)
        {
            check = new_check;
            CheckChanged?.Invoke(check);
        }
    }
}
