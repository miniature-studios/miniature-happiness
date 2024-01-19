using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(
        fileName = "ProjectedTilesSettings",
        menuName = "Settings/Create ProjectedTilesSettings",
        order = 6
    )]
    public class ProjectedTilesSettings : ScriptableObject
    {
        [SerializeField]
        private int projectedCount = 10;
        public int ProjectedCount => projectedCount;
    }
}
