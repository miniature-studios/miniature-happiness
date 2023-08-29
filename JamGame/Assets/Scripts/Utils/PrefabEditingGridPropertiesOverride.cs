#if UNITY_EDITOR
using TileUnion;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Utils
{
    [InitializeOnLoad]
    public static class PrefabEditingGridPropertiesOverride
    {
        static PrefabEditingGridPropertiesOverride()
        {
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
        }

        private static void OnPrefabStageOpened(PrefabStage prefabStage)
        {
            TileUnionImpl tileUnion =
                prefabStage.prefabContentsRoot.gameObject.GetComponent<TileUnionImpl>();
            if (tileUnion != null)
            {
                tileUnion.SetBuilderMatrix(GlobalGameSettings.GetGridProperties());
            }
        }
    }
}
#endif