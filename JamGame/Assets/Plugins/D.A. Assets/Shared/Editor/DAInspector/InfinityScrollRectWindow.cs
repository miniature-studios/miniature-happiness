using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.Shared
{
    public delegate void DrawItem<T>(T item);

    public class InfinityScrollRectWindow<T>
    {
        private List<T> items = new List<T>();
        private Vector2 scrollPosition;
        protected int visibleItemCount;
        protected float itemHeight;
        private float totalScrollHeight;
        private float visibleAreaHeight;
        private DrawItem<T> drawItem;

        public InfinityScrollRectWindow(int visibleItemCount, float itemHeight)
        {
            this.visibleItemCount = visibleItemCount;
            this.itemHeight = itemHeight;
        }

        public void SetData(List<T> items, DrawItem<T> drawItem)
        {
            this.drawItem = drawItem;
            this.items = items;

            if (items.Count < visibleItemCount)
            {
                visibleItemCount = items.Count;
            }

            visibleAreaHeight = visibleItemCount * itemHeight;
            totalScrollHeight = items.Count * itemHeight;
        }

        public void OnGUI()
        {
            if (items.Count == 0)
            {
                GUILayout.Label("No data.");
                return;
            }

            if (drawItem == null)
                return;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(visibleAreaHeight));

            float currentScrollPos = scrollPosition.y;
            int startIndex = Mathf.Max(0, (int)(currentScrollPos / itemHeight) - 1);
            int endIndex = Mathf.Min(items.Count, startIndex + visibleItemCount + 2);

            GUILayout.BeginVertical();
            GUILayout.Space(startIndex * itemHeight);

            for (int i = startIndex; i < endIndex; i++)
            {
                drawItem(items[i]);
            }

            GUILayout.Space(totalScrollHeight - endIndex * itemHeight);
            GUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }
    }
}
