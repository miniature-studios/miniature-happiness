using Sirenix.OdinInspector;
using TileBuilder;
using UnityEngine;

namespace Utils
{
    [CreateAssetMenu(
        fileName = "GameSettings",
        menuName = "GameSettings/CreateGameSettings",
        order = 5
    )]
    public class GameSettings : ScriptableObject
    {
        [SerializeField]
        [ReadOnly]
        private GridProperties gridProperties;
        public GridProperties Matrix => gridProperties;

        public void SetMatrix(GridProperties gridProperties)
        {
            this.gridProperties = gridProperties;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
