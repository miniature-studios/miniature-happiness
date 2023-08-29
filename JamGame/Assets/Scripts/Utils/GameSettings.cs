using Common;
using TileBuilder;
using UnityEngine;

namespace Utils
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings/CreateGameSettings", order = 5)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField]
        [InspectorReadOnly]
        private GridProperties matrix;
        public GridProperties Matrix => matrix;

        public void SetMatrix(GridProperties matrix)
        {
            this.matrix = matrix;
        }
    }
}
