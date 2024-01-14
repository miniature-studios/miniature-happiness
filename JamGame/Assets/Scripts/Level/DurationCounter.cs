using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level
{
    public struct DaysLived
    {
        public int Value;
    }

    [AddComponentMenu("Scripts/Level/Level.DurationCounter")]
    public class DurationCounter : MonoBehaviour
    {
        private DataProvider<DaysLived> daysLivedDataProvider;

        [ReadOnly]
        [SerializeField]
        private int daysLived = 0;

        private void Start()
        {
            daysLivedDataProvider = new DataProvider<DaysLived>(
                () => new DaysLived() { Value = daysLived }
            );
        }

        // Called by executor when day is end.
        public void DayEnds()
        {
            daysLived++;
        }
    }
}
