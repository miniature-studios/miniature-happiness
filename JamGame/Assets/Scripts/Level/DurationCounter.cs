using Common;
using Level.GlobalTime;
using UnityEngine;

namespace Level
{
    public struct DaysLived
    {
        public int Days;
        public Days GeneralInGameDuration;
    }

    [AddComponentMenu("Scripts/Level.DurationCounter")]
    public class DurationCounter : MonoBehaviour, IDataProvider<DaysLived>
    {
        [SerializeField]
        [InspectorReadOnly]
        private int daysLived = 0;

        [SerializeField]
        [InspectorReadOnly]
        private Days generalInGameDuration = new();

        // Called by executor when time has passed.
        public void TimeGone(Days time)
        {
            generalInGameDuration += time;
        }

        // Called by executor when day is end.
        public void DayEnds()
        {
            daysLived++;
        }

        public DaysLived GetData()
        {
            return new DaysLived()
            {
                Days = daysLived,
                GeneralInGameDuration = generalInGameDuration
            };
        }
    }
}
