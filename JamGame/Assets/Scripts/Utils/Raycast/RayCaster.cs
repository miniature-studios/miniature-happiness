using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils.Raycast
{
    public static class Raycaster
    {
        public static bool PointerIsOverUI(Vector2 screenPosition)
        {
            IEnumerable<GameObject> hitObjects = UIRaycast(ScreenPosToPointerData(screenPosition));
            return hitObjects.Any(x => x.GetComponentInParent<Blocker>() != null);
        }

        public static IEnumerable<GameObject> UIRaycast(Vector2 screenPos)
        {
            return UIRaycast(ScreenPosToPointerData(screenPos));
        }

        public static IEnumerable<GameObject> MaskedUIRaycast(Vector2 screenPos, LayerMask mask)
        {
            return UIRaycast(ScreenPosToPointerData(screenPos))
                .Where(x => ((1 << x.layer) & mask) != 0);
        }

        private static IEnumerable<GameObject> UIRaycast(PointerEventData pointerData)
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
