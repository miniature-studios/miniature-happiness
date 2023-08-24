#if UNITY_EDITOR
using Level.Room;
using TileUnion;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    internal class RoomCoreModelsPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload
        )
        {
            foreach (string str in importedAssets)
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(str);
                if (asset != null)
                {
                    CoreModel coreModel = asset.GetComponent<CoreModel>();
                    if (coreModel != null && coreModel.HashCode == "")
                    {
                        coreModel.SetHashCode();
                        EditorUtility.SetDirty(asset);
                    }

                    Level.Inventory.Room.View inventoryView =
                        asset.GetComponent<Level.Inventory.Room.View>();
                    if (inventoryView != null)
                    {
                        if (inventoryView.CoreModelPrefab == null)
                        {
                            Debug.LogError($"No CoreModelPrefab link in {asset.name}");
                        }
                        else
                        {
                            inventoryView.HashCode = inventoryView.CoreModelPrefab.HashCode;
                            EditorUtility.SetDirty(asset);
                        }
                    }

                    Level.Shop.Room.View shopView = asset.GetComponent<Level.Shop.Room.View>();
                    if (shopView != null)
                    {
                        if (shopView.CoreModelPrefab == null)
                        {
                            Debug.LogError($"No CoreModelPrefab link in {asset.name}");
                        }
                        else
                        {
                            shopView.HashCode = shopView.CoreModelPrefab.HashCode;
                            EditorUtility.SetDirty(asset);
                        }
                    }

                    TileUnionImpl tileUnion = asset.GetComponent<TileUnionImpl>();
                    if (tileUnion != null)
                    {
                        if (tileUnion.CoreModelPrefab == null)
                        {
                            Debug.LogError($"No CoreModelPrefab link in {asset.name}");
                        }
                        else
                        {
                            tileUnion.HashCode = tileUnion.CoreModelPrefab.HashCode;
                            EditorUtility.SetDirty(asset);
                        }
                    }
                }
            }
        }
    }
}
#endif
