using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.Shared
{
    /// <summary>
    /// https://stackoverflow.com/a/71288271
    /// </summary>
    public static class GameViewUtils
    {
        private static readonly object gameViewSizesInstance;
        private static readonly MethodInfo getGroup;
        private const string gameViewSizesType = "UnityEditor.GameViewSizes";
        private const string gameViewType = "UnityEditor.GameView";
        static GameViewUtils()
        {
            Type sizesType = typeof(Editor).Assembly.GetType(gameViewSizesType);
            Type singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            PropertyInfo instanceProp = singleType.GetProperty("instance");
            getGroup = sizesType.GetMethod("GetGroup");
            gameViewSizesInstance = instanceProp.GetValue(null, null);
        }
        public static bool SetGameViewSize(Vector2 size)
        {
            try
            {
                GameViewSizeGroupType groupType = GetCurrentGroup();

                bool exists = SizeExists(groupType, (int)size.x, (int)size.y);

                int index;

                if (exists)
                {
                    index = FindSize(groupType, (int)size.x, (int)size.y);
                }
                else
                {
                    index = AddCustomSize(
                        GameViewSizeType.FixedResolution,
                        groupType,
                        (int)size.x,
                        (int)size.y,
                        $"{(int)size.x}x{(int)size.y}");
                }

                SetSize(index);

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// http://answers.unity.com/answers/192818/view.html
        /// </summary>
        public static bool GetGameViewSize(out Vector2 size)
        {
            size = new Vector2(0, 0);

            try
            {
                Type gvWndType = typeof(Editor).Assembly.GetType(gameViewType);

                MethodInfo sizeOfMainGameView = gvWndType.GetMethod(
                    "GetSizeOfMainGameView",
                    BindingFlags.NonPublic | BindingFlags.Static);

                size = (Vector2)sizeOfMainGameView.Invoke(null, null);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }
        public static GameViewSizeGroupType GetCurrentGroup()
        {
            Type sizesType = typeof(Editor).Assembly.GetType(gameViewSizesType);

            PropertyInfo currentGroupType = sizesType.GetProperty(
                "currentGroupType",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            return (GameViewSizeGroupType)currentGroupType.GetValue(null, null);
        }

        public static int AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string resolutionName)
        {
            object group = GetGroup(sizeGroupType);
            MethodInfo addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize");

            Assembly assembly = typeof(Editor).Assembly;
            Type gameViewSize = assembly.GetType("UnityEditor.GameViewSize");
            Type gameViewSizeType = assembly.GetType("UnityEditor.GameViewSizeType");
            ConstructorInfo ctor = gameViewSize.GetConstructor(new[] {
            gameViewSizeType,
            typeof(int),
            typeof(int),
            typeof(string)
        });
            object newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, resolutionName });
            addCustomSize.Invoke(group, new[] { newSize });

            return FindSize(sizeGroupType, width, height);
        }

        public static void SetSize(int index)
        {
            Type gvWndType = typeof(Editor).Assembly.GetType(gameViewType);

            PropertyInfo selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            EditorWindow gvWnd = EditorWindow.GetWindow(gvWndType);

            selectedSizeIndexProp.SetValue(gvWnd, index, null);
        }
        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            return FindSize(sizeGroupType, width, height) != -1;
        }
        public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
            // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
            // iterate through the sizes via group.GetGameViewSize(int index)

            object group = GetGroup(sizeGroupType);
            Type groupType = group.GetType();
            MethodInfo getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            MethodInfo getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            MethodInfo getGameViewSize = groupType.GetMethod("GetGameViewSize");
            Type gvsType = getGameViewSize.ReturnType;
            PropertyInfo widthProp = gvsType.GetProperty("width");
            PropertyInfo heightProp = gvsType.GetProperty("height");
            object[] indexValue = new object[1];

            for (int i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;

                object size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);

                if (sizeWidth == width && sizeHeight == height)
                {
                    return i;
                }
            }

            return -1;
        }

        private static object GetGroup(GameViewSizeGroupType type)
        {
            return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
        }
    }
    public enum GameViewSizeType
    {
        AspectRatio,
        FixedResolution
    }
}