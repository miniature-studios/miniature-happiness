#if UNITY_EDITOR
using TileUnion;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Utils
{
    [InitializeOnLoad]
    public class MatrixSetter
    {
        static MatrixSetter()
        {
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
        }

        private static void OnPrefabStageOpened(PrefabStage prefabStage)
        {
            TileUnionImpl tileUnion =
                prefabStage.prefabContentsRoot.gameObject.GetComponent<TileUnionImpl>();
            if (tileUnion != null)
            {
                tileUnion.SetBuilderMatrix(GlobalGameSettings.GetMatrix());
            }
        }
    }
}
#endif