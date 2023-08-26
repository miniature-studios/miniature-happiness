using Common;
using TileBuilder;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings/CreateGameSettings", order = 5)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField]
        [InspectorReadOnly]
        private Matrix matrix;
        public Matrix Matrix => matrix;

        public void SetMatrix(Matrix matrix)
        {
            this.matrix = matrix;
            EditorUtility.SetDirty(this);
        }
    }
}
