using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level
{
    public struct DaysLived
    {
        public int Value;
    }

    [AddComponentMenu("Scripts/Level.DurationCounter")]
    public class DurationCounter : MonoBehaviour, IDataProvider<DaysLived>
    {
        [ReadOnly]
        [SerializeField]
        private int daysLived = 0;

        // Called by executor when day is end.
        public void DayEnds()
        {
            daysLived++;
        }

        public DaysLived GetData()
        {
            return new DaysLived()
            {
                Value = daysLived
            };
        }
    }
}
