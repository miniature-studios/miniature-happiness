#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace Utils
{
    [InitializeOnLoad]
    public static class GridPropertiesMenu
    {
        static GridPropertiesMenu()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            string[] paths = AssetDatabase.FindAssets("t:GridProperties");
            List<GridProperties> gridProperties = paths
                .Select(x =>
                    AssetDatabase.LoadAssetAtPath<GridProperties>(AssetDatabase.GUIDToAssetPath(x))
                )
                .ToList();

            int index = gridProperties.IndexOf(GlobalGameSettings.GetGridProperties());

            int selected = EditorGUILayout.Popup(
                "Default GridProperties:",
                index,
                gridProperties.Select(x => x.name).ToArray(),
                GUILayout.Width(300)
            );
            if (selected >= 0 && GlobalGameSettings.GetGridProperties() != gridProperties[selected])
            {
                GlobalGameSettings.SetGridProperties(gridProperties[selected]);
            }
        }
    }
}
#endif
