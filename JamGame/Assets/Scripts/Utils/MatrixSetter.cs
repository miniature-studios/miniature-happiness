using TileUnion;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

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
            Debug.Log("OnPrefabStageOpened " + prefabStage.assetPath);

            TileUnionImpl tileUnion = prefabStage.prefabContentsRoot.gameObject.GetComponent<TileUnionImpl>();
            if (tileUnion != null)
            {
                tileUnion.SetBuilderMatrix(GlobalGameSettings.GetMatrix());
            }
        }
    }
}
