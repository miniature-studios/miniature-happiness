#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryController))]
public class InventoryControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InventoryController inventory_controller = serializedObject.targetObject as InventoryController;

        foreach (NamedRoomInventoriUI item in inventory_controller.namedRoomInventoryUIs)
        {
            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(item.Name))
            {
                inventory_controller.CreateOneUI(item.RIUI);
            }
            EditorGUILayout.EndHorizontal();
        }
        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add gods inventory"))
        {
            inventory_controller.CreateGodsInventory();
        }
        EditorGUILayout.EndHorizontal();

        _ = DrawDefaultInspector();

        _ = serializedObject.ApplyModifiedProperties();
    }
}
#endif