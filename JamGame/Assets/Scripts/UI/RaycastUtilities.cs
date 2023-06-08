using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

// For checking if clicked on ui in future
public static class RaycastUtilities
{
    public static bool PointerIsOverUI(Vector2 screenPos)
    {
        GameObject hitObject = UIRaycast(ScreenPosToPointerData(screenPos))?.First();
        return hitObject != null && hitObject.layer == LayerMask.NameToLayer("UI");
    }

    public static IEnumerable<GameObject> UIRaycast(Vector2 screenPos)
    {
        return UIRaycast(ScreenPosToPointerData(screenPos));
    }

    private static IEnumerable<GameObject> UIRaycast(PointerEventData pointerData)
    {
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count < 1 ? null : results.Select(x => x.gameObject);
    }

    private static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
    {
        return new(EventSystem.current) { position = screenPos };
    }
}
