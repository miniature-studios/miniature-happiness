using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(
        fileName = "ProjectedTilesSettings",
        menuName = "Settings/Create ProjectedTilesSettings"
    )]
    public class ProjectedTilesSettings : ScriptableObject
    {
        [SerializeField]
        private int projectedCount = 10;
        public int ProjectedCount => projectedCount;
    }
}
