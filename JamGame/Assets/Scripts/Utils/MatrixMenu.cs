using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using UnityEditor;
using UnityToolbarExtender;

namespace Utils
{
    [InitializeOnLoad]
    public static class MatrixMenu
    {
        static MatrixMenu()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            string[] paths = AssetDatabase
                .FindAssets("t:Matrix");
            List<Matrix> matrixes =
                paths.Select(x => AssetDatabase.LoadAssetAtPath<Matrix>(AssetDatabase.GUIDToAssetPath(x)))
                .ToList();

            int index = matrixes.IndexOf(GlobalGameSettings.GetMatrix());

            int selected = EditorGUILayout.Popup(index, matrixes.Select(x => x.name).ToArray());
            if (selected >= 0 && GlobalGameSettings.GetMatrix() != matrixes[selected])
            {
                GlobalGameSettings.SetMatrix(matrixes[selected]);
            }
        }
    }
}
