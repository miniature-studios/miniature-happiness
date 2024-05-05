using UnityEditor;
using UnityEngine;

public class GUISplitter : EditorWindow
{
    Vector2 posLeft;
    Vector2 posRight;
    GUIStyle styleLeftView;
    GUIStyle styleRightView;
    float splitterPos;
    Rect splitterRect;
    Vector2 dragStartPos;
    bool dragging;
    float splitterWidth = 5;

    // Add menu named "My Window" to the Window menu
    [MenuItem("GUI/GUISplitter")]
    static void Init()
    {
        GUISplitter window = (GUISplitter)EditorWindow.GetWindow(
            typeof(GUISplitter));
        window.position = new Rect(200, 200, 200, 200);
        window.splitterPos = 100;
    }

    void OnGUI()
    {
        if (styleLeftView == null)
            styleLeftView = new GUIStyle(GUI.skin.box);
        if (styleRightView == null)
            styleRightView = new GUIStyle(GUI.skin.button);

        GUILayout.BeginHorizontal();

        // Left view
        posLeft = GUILayout.BeginScrollView(posLeft,
            GUILayout.Width(splitterPos),
            GUILayout.MaxWidth(splitterPos),
            GUILayout.MinWidth(splitterPos));
        GUILayout.Box("Left View",
                styleLeftView,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();

        // Splitter
        GUILayout.Box("",
            GUILayout.Width(splitterWidth),
            GUILayout.MaxWidth(splitterWidth),
            GUILayout.MinWidth(splitterWidth),
            GUILayout.ExpandHeight(true));
        splitterRect = GUILayoutUtility.GetLastRect();

        // Right view
        posRight = GUILayout.BeginScrollView(posRight,
            GUILayout.ExpandWidth(true));
        GUILayout.Box("Right View",
        styleRightView,
        GUILayout.ExpandWidth(true),
        GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();

        GUILayout.EndHorizontal();

        // Splitter events
        if (Event.current != null)
        {
            switch (Event.current.rawType)
            {
                case EventType.MouseDown:
                    if (splitterRect.Contains(Event.current.mousePosition))
                    {
                        Debug.Log("Start dragging");
                        dragging = true;
                    }
                    break;
                case EventType.MouseDrag:
                    if (dragging)
                    {
                        Debug.Log("moving splitter");
                        splitterPos += Event.current.delta.x;
                        Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (dragging)
                    {
                        Debug.Log("Done dragging");
                        dragging = false;
                    }
                    break;
            }
        }
    }
}