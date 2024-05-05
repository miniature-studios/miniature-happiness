using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class ContextMenuItems
    {
        [MenuItem("GameObject/Tools/" + FcuConfig.ProductName + "/" + FcuConfig.DestroyChilds, false, 0)]
        private static void DestroyChilds_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponent(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.DestroyChilds_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }

        [MenuItem("GameObject/Tools/" + FcuConfig.ProductName + "/" + FcuConfig.SetFcuToSyncHelpers, false, 1)]
        private static void SetFcuToSyncHelpers_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponent(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.SetFcuToSyncHelpers_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }

        [MenuItem("GameObject/Tools/" + FcuConfig.ProductName + "/" + FcuConfig.OptimizeSyncHelpers, false, 1)]
        private static void OptimizeSyncHelpers_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponent(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.OptimizeSyncHelpers_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }

        [MenuItem("GameObject/Tools/" + FcuConfig.ProductName + "/" + FcuConfig.CompareTwoObjects, false, 2)]
        private static void Compare_OnClick()
        {
            if (Selection.gameObjects.Length != 2)
            {
                DALogger.LogError(FcuLocKey.log_incorrect_selection.Localize());
                return;
            }

            GameObject go1 = Selection.gameObjects[0];
            GameObject go2 = Selection.gameObjects[1];

            bool e1 = go1.TryGetComponent(out SyncHelper sh1);
            bool e2 = go2.TryGetComponent(out SyncHelper sh2);

            if (e1 == false)
            {
                DALogger.LogError(FcuLocKey.log_no_sync_helper.Localize(go1.name));
                return;
            }

            if (e2 == false)
            {
                DALogger.LogError(FcuLocKey.log_no_sync_helper.Localize(go2.name));
                return;
            }

            ComparerWindow.Show(sh1, sh2);
        }

        [MenuItem("GameObject/Tools/" + FcuConfig.ProductName + "/" + FcuConfig.DestroyLastImported, false, 3)]
        private static void DestroyLastImportedFrames_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponent(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.DestroyLastImportedFrames_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }

        [MenuItem("GameObject/Tools/" + FcuConfig.ProductName + "/" + FcuConfig.DestroySyncHelpers, false, 4)]
        private static void DestroySyncHelpers_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponent(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.DestroySyncHelpers_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }

        [MenuItem("GameObject/Tools/" + FcuConfig.ProductName + "/" + FcuConfig.CreatePrefabs, false, 5)]
        private static void CreatePrefabs_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponent(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.CreatePrefabs_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }

        [MenuItem("GameObject/Tools/" + FcuConfig.ProductName + "/" + FcuConfig.ResetToPrefabState, false, 6)]
        private static void ResetToPrefabState_OnClick()
        {
            GameObject selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject == null)
            {
                DALogger.LogError(FcuLocKey.log_gameobject_not_selected_in_hierarchy.Localize(nameof(GameObject)));
                return;
            }

            PrefabUtility.RevertPrefabInstance(Selection.activeGameObject, InteractionMode.AutomatedAction);

            DALogger.Log(FcuLocKey.log_object_reset.Localize(selectedGameObject.name));
        }

        [MenuItem("GameObject/Tools/" + FcuConfig.ProductName + "/" + FcuConfig.ResetAllComponents, false, 7)]
        private static void ResetAllComponents_OnClick()
        {
            GameObject selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject == null)
            {
                DALogger.LogError(FcuLocKey.log_gameobject_not_selected_in_hierarchy.Localize(nameof(GameObject)));
                return;
            }

            Component[] components = selectedGameObject.GetComponents<Component>();

            if (components.IsEmpty())
            {
                DALogger.LogError(FcuLocKey.log_no_components_in_gameobject.Localize(selectedGameObject.name));
                return;
            }

            int count = 0;

            foreach (var item in components)
            {
                SerializedObject serializedObject = new SerializedObject(item);
                SerializedProperty propertyIterator = serializedObject.GetIterator();

                while (propertyIterator.NextVisible(true))
                {
                    PrefabUtility.RevertPropertyOverride(propertyIterator, InteractionMode.AutomatedAction);
                    count++;
                }
            }

            DALogger.Log(FcuLocKey.log_props_reset.Localize(count));
        }

        [MenuItem("Tools/" + FcuConfig.ProductName + "/" + FcuConfig.Create + " " + FcuConfig.ProductName, false, 0)]
        private static void CreateFcu_OnClick()
        {
            FcuEventHandlers.CreateFcu_OnClick();
        }
    }
}