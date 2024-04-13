using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level
{
    public struct DaysLived
    {
        public int Value;
    }

    [AddComponentMenu("Scripts/Level/Level.DayCounter")]
    public class DayCounter : MonoBehaviour
    {
        private DataProvider<DaysLived> daysLivedDataProvider;

        [ReadOnly]
        [SerializeField]
        private int daysLived = 0;

        private void Start()
        {
            daysLivedDataProvider = new DataProvider<DaysLived>(
                () => new DaysLived() { Value = daysLived },
                DataProviderServiceLocator.ResolveType.Singleton
            );
        }

        // Called by executor when day is end.
        public void DayEnd()
        {
            daysLived++;
        }
    }
}
