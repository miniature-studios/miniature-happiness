using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common
{
    public static class RayCastUtilities
    {
        public static bool PointerIsOverUI(Vector2 screenPosition)
        {
            IEnumerable<GameObject> hitObjects = UIRayCast(ScreenPosToPointerData(screenPosition));
            return hitObjects.Any(x => x.layer == LayerMask.NameToLayer("UI"));
        }

        public static IEnumerable<GameObject> UIRayCast(Vector2 screenPos)
        {
            return UIRayCast(ScreenPosToPointerData(screenPos));
        }

        private static IEnumerable<GameObject> UIRayCast(PointerEventData pointerData)
        {
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointerData, results);
            return results.Select(x => x.gameObject);
        }

        private static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
        {
            return new(EventSystem.current) { position = screenPos };
        }
    }
}
